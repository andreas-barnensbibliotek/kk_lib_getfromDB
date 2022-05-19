using KulturkatalogenDomain.Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace kk_lib_getFromDB
{

    public class clsPublicKatalogSearch
    {
        /// <summary>
        /// Gör en sökning i databasen. Om clsSearchInput.UtovareID = 0, så körs en vanlig sökning. Annars hämtas alla arr av utövaren med medskickat ID.
        /// Exempel: 
        /// ClsPublicSearchCmdInfo searchobj = new ClsPublicSearchCmdInfo();Adde
        /// searchobj.ArrTypID = 0;
        /// searchobj.FreeTextSearch = "";
        /// searchobj.CmdTyp = "";
        /// searchobj.AgeSpans = new List<int> { };
        /// searchobj.KonstartIDs = new List<int> { 2 };
        /// searchobj.ConnectionString = connString;
        /// searchobj.Tags = new List<string> { "" };
        /// searchobj.maxResults = 10;
        /// searchobj.IsPublic = null;
        /// //searchobj.UtovareID = 1014;
        /// clsPublicKatalogSearch query = new();
        /// IEnumerable<PublicSearchReturnJsonInfo> ieQueryResult = query.DoSearch(searchobj);
        /// </summary>
        /// <param name="clsSearchInput">En samling sökparametrar som samlas i en ClsPublicSearchCmdInfo</param>
        /// <param name="getExtendedResults">Om true, så hämtas även utökad data om utövare, fakta och media</param>
        /// <returns>En IEnumerable av objekt av typen PublicSearchReturnJsonInfo</returns>
        public IEnumerable<PublicSearchReturnJsonInfo> DoSearch(ClsPublicSearchCmdInfo clsSearchInput, bool getExtendedResults = false)
        {
            IEnumerable<PublicSearchReturnJsonInfo> searchResult = MainSearch(clsSearchInput, getExtendedResults);
            return searchResult;
        }

        /// <summary>
        /// Gör en snabb sökning i kolumnerna Rubrik och utövare. Matchar om SearchText är en del av innehållet
        /// Exempel: 
        /// ClsPublicSearchAutocomplete qryParams = new()
        /// {
        ///         SearchText = "kapt",
        ///         MaxResults = 10,
        ///         ConnectionString = connString
        ///    };
        /// </summary>
        /// <param name="clsSearchInput">En samling sökparametrar som samlas i en ClsPublicSearchAutocomplete</param>
        /// <returns>En IEnumerable av objekt av typen ClsPublicSearchInfo</returns>
        public static IEnumerable<PublicSearchReturnJsonInfo> DoAutoCompleteSearch(ClsPublicSearchAutocomplete clsSearchInput)
        {
            IEnumerable<PublicSearchReturnJsonInfo> searchResult = AutoCompleteSearch(clsSearchInput);
            return searchResult;
        }

        private static IEnumerable<PublicSearchReturnJsonInfo> MainSearch(ClsPublicSearchCmdInfo clsSearchInput, bool getExtendedResults)
        {
            //try
            //{
            //Skapa och fyll datatables med de parametrar som kommer in i form av arrayer.
            //------------------------------------------------------------
            DataTable konstartDt = new();
            konstartDt.Columns.Add("konstart", typeof(int));
            foreach (int konstart in clsSearchInput.KonstartIDs)
            {
                konstartDt.Rows.Add(konstart);
            }

            DataTable ageDt = new();
            ageDt.Columns.Add("ageSpanID", typeof(int));
            foreach (int agespanID in clsSearchInput.AgeSpans)
            {
                ageDt.Rows.Add(agespanID);
            }

            DataTable tagsDt = new();
            tagsDt.Columns.Add("tag", typeof(string));
            foreach (string tag in clsSearchInput.Tags)
            {
                tagsDt.Rows.Add(tag);
            }
            //--------------------------------------------------------------

            string procedure = "kk_aj_proc_Search_v3";
            using IDbConnection db = new SqlConnection(clsSearchInput.ConnectionString);
            db.Open();
            var @params = new
            {
                arrtypid = clsSearchInput.ArrTypID,
                konstartTblIDs = konstartDt,
                ageLimitsTblInts = ageDt,
                pubyesno = clsSearchInput.IsPublic,
                tagsTblStrings = tagsDt,
                freetext = clsSearchInput.FreeTextSearch,
                maxResults = clsSearchInput.maxResults,
                utovareID = clsSearchInput.UtovareID
            };
            IEnumerable<PublicSearchReturnJsonInfo> returnValue = db.Query<PublicSearchReturnJsonInfo>(procedure, @params, commandType: CommandType.StoredProcedure);

            //Lägg till listor med utövare, fakta och media
            //if (getExtendedResults)
            //{
            foreach (PublicSearchReturnJsonInfo responseRecord in returnValue)
            {
                responseRecord.ansokningMediaImage = new mediaInfo();
                responseRecord.ansokningMediaImage.MediaUrl = responseRecord.ImageUrl;

                if (getExtendedResults)
                {
                    responseRecord.ansokningUtovardata = GetUtovareInfo((int)responseRecord.ansokningUtovarid, clsSearchInput.ConnectionString);
                    responseRecord.ansokningFaktalist = GetFaktaInfo((int)responseRecord.ansokningid, clsSearchInput.ConnectionString);
                    responseRecord.ansokningMedialist = GetMediaInfo((int)responseRecord.ansokningid, clsSearchInput.ConnectionString);
                }
                else
                {
                    responseRecord.ansokningUtovardata = new();
                    responseRecord.ansokningFaktalist = new List<faktainfo>();
                    responseRecord.ansokningMedialist = new();
                }

            }
            //} else
            //{
            //    foreach (PublicSearchReturnJsonInfo responseRecord in returnValue)
            //    {
            //        responseRecord.ansokningUtovardata = new();
            //        responseRecord.ansokningFaktalist = new List<faktainfo>();
            //        responseRecord.ansokningMedialist = new List<mediaInfo>();
            //    }
            //}

            return returnValue;
        }

        private static IEnumerable<PublicSearchReturnJsonInfo> AutoCompleteSearch(ClsPublicSearchAutocomplete clsSearchInput)
        {
            string procedure = "kk_aj_proc_autocompleteSearch_v2";
            using IDbConnection db = new SqlConnection(clsSearchInput.ConnectionString);
            db.Open();
            var @params = new { searchText = clsSearchInput.SearchText, maxResults = clsSearchInput.MaxResults };
            IEnumerable<PublicSearchReturnJsonInfo> returnValue = db.Query<PublicSearchReturnJsonInfo>(procedure, @params, commandType: CommandType.StoredProcedure);

            return returnValue;
        }


        private static List<faktainfo> GetFaktaInfo(int arrID, string ConnectionString)
        {
            string procedure = "kk_aj_proc_getfaktabyarrid";
            using IDbConnection db = new SqlConnection(ConnectionString);
            db.Open();
            var @params = new { ArrID = arrID };
            var results = db.Query<faktainfo>(procedure, @params, commandType: CommandType.StoredProcedure);
            List<faktainfo> returnValue = new List<faktainfo>(results);

            return returnValue;
        }

        private static utovareInfo GetUtovareInfo(int UtovarID, string ConnectionString)
        {
            string procedure = "kk_aj_proc_getUtovareById";
            using IDbConnection db = new SqlConnection(ConnectionString);
            db.Open();
            var @params = new { UtovarID = UtovarID };
            utovareInfo returnValue = db.QuerySingle<utovareInfo>(procedure, @params, commandType: CommandType.StoredProcedure);

            return returnValue;
        }

        private static List<mediaInfo> GetMediaInfo(int arrID, string ConnectionString)
        {
            string procedure = "kk_aj_proc_GetMediaByArrid";
            using IDbConnection db = new SqlConnection(ConnectionString);
            db.Open();
            var @params = new { ArrID = arrID };
            var results = db.Query<mediaInfo>(procedure, @params, commandType: CommandType.StoredProcedure);
            List<mediaInfo> returnValue = new List<mediaInfo>(results);

            return returnValue;
        }

        private static List<ClsPublicSearchInfo> FillResultList__OLD(DataTable dt, string connectionString, bool minimumResult)
        {
            List<ClsPublicSearchInfo> lstReturnValue = new();

            foreach (DataRow row in dt.Rows)
            {
                //Skapa ett objekt per rad i svaret, och fyll det med innehållet i varje kolumn
                ClsPublicSearchInfo rowObject = new();

                rowObject.ArkivStatus = (int?)row["ArkivStatus"];
                rowObject.ArrangemangstypID = (int?)row["ArrangemangstypID"];
                rowObject.ArrID = (int)row["ArrID"];
                rowObject.Datum = (DateTime?)row["Datum"];
                rowObject.ImageUrl = row["MediaU"].ToString();
                rowObject.Konstform = row["konstform"].ToString();
                rowObject.Konstform2 = (int?)row["konstform2"];
                rowObject.Konstform3 = (int?)row["konstform3"];
                rowObject.Organisation = row["Organisation"].ToString();
                rowObject.periodslut = row["periodslut"] == DBNull.Value ? null : (DateTime?)row["periodslut"];
                rowObject.periodstart = row["periodstart"] == DBNull.Value ? null : (DateTime?)row["periodstart"];
                rowObject.Publicerad = row["Publicerad"].ToString();
                rowObject.Rubrik = row["Rubrik"].ToString();
                rowObject.Startyear = row["startyear"].ToString();
                rowObject.Stoppyear = row["stoppyear"].ToString();
                rowObject.MinAge = row["minage"].ToString();
                rowObject.MaxAge = row["maxage"].ToString();
                rowObject.Underrubrik = row["Underrubrik"].ToString();
                rowObject.UtovarID = (int?)row["UtovarID"];
                rowObject.ListFilterFaktaInfo = new List<filterfaktaInfo>();
                if (minimumResult)
                {
                    rowObject.ListFaktaInfo = new List<faktainfo>();
                    rowObject.ListUtovareInfo = new List<utovareInfo>();
                    rowObject.ListMediaInfo = new List<mediaInfo>();
                }
                else
                {
                    rowObject.ListFaktaInfo = GetFaktaInfo((int)row["ArrID"], connectionString);
                    //rowObject.ListUtovareInfo = GetUtovareInfo((int)row["UtovarID"], connectionString);
                    rowObject.ListMediaInfo = GetMediaInfo((int)row["ArrID"], connectionString);
                }

                lstReturnValue.Add(rowObject);
            }

            return lstReturnValue;
        }

    }
}