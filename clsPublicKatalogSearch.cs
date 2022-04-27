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
        /// Gör en sökning i databasen
        /// Exempel: 
        /// ClsPublicSearchCmdInfo qryParams = new()
        /// {
        ///         ArrTypID = 0,
        ///         CmdTyp = "",
        ///         KonstartIDs = new List<int> { 2, 3 },
        ///         AgeSpans = new List<int> { 2,3 },
        ///         Tags = new List<string> { "foo","bar" },
        ///         IsPublic = "ja",
        ///         FreeTextSearch = "jkgdl" | null,
        ///         ConnectionString = connString
        ///    };
        /// </summary>
        /// <param name="clsSearchInput">En samling sökparametrar som samlas i en ClsPublicSearchCmdInfo</param>
        /// <returns>En IEnumerable av objekt av typen PublicSearchReturnJsonInfo</returns>
        public IEnumerable<PublicSearchReturnJsonInfo> DoSearch(ClsPublicSearchCmdInfo clsSearchInput)
        {
            IEnumerable<PublicSearchReturnJsonInfo> searchResult = MainSearch(clsSearchInput);
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
        /// <returns>En IEnumerable av objekt av typen PublicSearchReturnJsonInfo</returns>
        public static IEnumerable<PublicSearchReturnJsonInfo> DoAutoCompleteSearch(ClsPublicSearchAutocomplete clsSearchInput)
        {
            IEnumerable<PublicSearchReturnJsonInfo> searchResult = AutoCompleteSearch(clsSearchInput);
            return searchResult;
        }

        private static IEnumerable<PublicSearchReturnJsonInfo> MainSearch(ClsPublicSearchCmdInfo clsSearchInput)
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

            string procedure = "kk_aj_proc_Search_v2";
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
                maxResults = clsSearchInput.maxResults
            };
            IEnumerable<ClsPublicSearchInfo> returnValue = db.Query<ClsPublicSearchInfo>(procedure, @params, commandType: CommandType.StoredProcedure);

            //Lägg till listor med utövare, fakta och media
            foreach (ClsPublicSearchInfo responseRecord in returnValue)
            {
                responseRecord.ListUtovareInfo = GetUtovareInfo((int)responseRecord.UtovarID, clsSearchInput.ConnectionString);
                responseRecord.ListFaktaInfo = GetFaktaInfo((int)responseRecord.ArrID, clsSearchInput.ConnectionString);
                responseRecord.ListMediaInfo = GetMediaInfo((int)responseRecord.ArrID, clsSearchInput.ConnectionString);
            }

            return returnValue;
        }

        private static List<ClsPublicSearchInfo> FillResultList(DataTable dt, string connectionString, bool minimumResult)
        {
            List<PublicSearchReturnJsonInfo> lstReturnValue = new();

            foreach (DataRow row in dt.Rows)
            {
                //Skapa ett objekt per rad i svaret, och fyll det med innehållet i varje kolumn
                ClsPublicSearchInfo rowObject = new();

                rowObject.ArkivStatus = (int?)row["ArkivStatus"];
                rowObject.ArrangemangstypID = (int?)row["ArrangemangstypID"];
                rowObject.ArrID = (int)row["ArrID"];
                rowObject.Datum = (DateTime?)row["Datum"];
                rowObject.ImageUrl = row["ImageUrl"].ToString();
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
                    rowObject.ListUtovareInfo = GetUtovareInfo((int)row["UtovarID"], connectionString);
                    rowObject.ListMediaInfo = GetMediaInfo((int)row["ArrID"], connectionString);
                }


                lstReturnValue.Add(rowObject);
            }

            return lstReturnValue;
        }

        private static IEnumerable<ClsPublicSearchInfo> AutoCompleteSearch(ClsPublicSearchAutocomplete clsSearchInput)
        {
            string procedure = "kk_aj_proc_autocompleteSearch";
            using IDbConnection db = new SqlConnection(clsSearchInput.ConnectionString);
            db.Open();
            var @params = new { searchText = clsSearchInput.SearchText, maxResults = clsSearchInput.MaxResults };
            IEnumerable<ClsPublicSearchInfo> returnValue = db.Query<ClsPublicSearchInfo>(procedure, @params, commandType: CommandType.StoredProcedure);

            return returnValue;
        }

        private static IEnumerable<faktainfo> GetFaktaInfo(int arrID, string ConnectionString)
        {
            string procedure = "kk_aj_proc_getfaktabyarrid";
            using IDbConnection db = new SqlConnection(ConnectionString);
            db.Open();
            var @params = new { ArrID = arrID };
            IEnumerable<faktainfo> returnValue = db.Query<faktainfo>(procedure, @params, commandType: CommandType.StoredProcedure);

            return returnValue;
        }

        private static IEnumerable<utovareInfo> GetUtovareInfo(int UtovarID, string ConnectionString)
        {
            string procedure = "kk_aj_proc_getUtovareById";
            using IDbConnection db = new SqlConnection(ConnectionString);
            db.Open();
            var @params = new { UtovarID = UtovarID };
            IEnumerable<utovareInfo> returnValue = db.Query<utovareInfo>(procedure, @params, commandType: CommandType.StoredProcedure);

            return returnValue;
        }

        private static IEnumerable<mediaInfo> GetMediaInfo(int arrID, string ConnectionString)
        {
            string procedure = "kk_aj_proc_GetMediaByArrid";
            using IDbConnection db = new SqlConnection(ConnectionString);
            db.Open();
            var @params = new { ArrID = arrID };
            IEnumerable<mediaInfo> returnValue = db.Query<mediaInfo>(procedure, @params, commandType: CommandType.StoredProcedure);

            return returnValue;
        }
    }
}
