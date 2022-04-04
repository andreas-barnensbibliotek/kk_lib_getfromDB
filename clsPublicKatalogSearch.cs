using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace kk_lib_getFromDB
{

    public class ClsQuery
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
        /// <returns>En IEnumerable av objekt av typen ClsPublicSearchInfo</returns>
        public IEnumerable<ClsPublicSearchInfo> DoSearch(ClsPublicSearchCmdInfo clsSearchInput)
        {
            IEnumerable<ClsPublicSearchInfo> searchResult = MainSearch(clsSearchInput);
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
        public static IEnumerable<ClsPublicSearchInfo> DoAutoCompleteSearch(ClsPublicSearchAutocomplete clsSearchInput)
        {
            IEnumerable<ClsPublicSearchInfo> searchResult = AutoCompleteSearch(clsSearchInput);
            return searchResult;
        }

        private static IEnumerable<ClsPublicSearchInfo> MainSearch(ClsPublicSearchCmdInfo clsSearchInput)
        {
            //try
            //{
            SqlConnection sqlConn = new(clsSearchInput.ConnectionString);

            sqlConn.Open();

            //Skapa och fyll datatables med de parametrar som kommer in i form av arrayer.
            //------------------------------------------------------------
            DataTable konstartDt = new();
            konstartDt.Columns.Add("konstart");
            foreach (int konstart in clsSearchInput.KonstartIDs)
            {
                DataRow dr;
                dr = konstartDt.NewRow();
                dr["konstart"] = konstart;
                konstartDt.Rows.Add(dr);
            }

            DataTable ageDt = new();
            ageDt.Columns.Add("ageSpanID");
            foreach (int agespanID in clsSearchInput.AgeSpans)
            {
                DataRow dr;
                dr = ageDt.NewRow();
                dr["ageSpanID"] = agespanID;
                ageDt.Rows.Add(dr);
            }

            DataTable tagsDt = new();
            tagsDt.Columns.Add("tag");
            foreach (string tag in clsSearchInput.Tags)
            {
                DataRow dr;
                dr = tagsDt.NewRow();
                dr["tag"] = tag;
                tagsDt.Rows.Add(dr);
            }
            //--------------------------------------------------------------

            // Configure the SqlCommand and SqlParameter.  
            SqlCommand cmdSearch = new("kk_aj_proc_Search_v2", sqlConn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmdSearch.Parameters.Add("@arrtypid", SqlDbType.Int).Value = clsSearchInput.ArrTypID;
            cmdSearch.Parameters.Add("@konstartTblIDs", SqlDbType.Structured).Value = konstartDt;
            cmdSearch.Parameters.Add("@ageLimitsTblInts", SqlDbType.Structured).Value = ageDt;
            cmdSearch.Parameters.Add("@pubyesno", SqlDbType.NVarChar, 3).Value = clsSearchInput.IsPublic;
            cmdSearch.Parameters.Add("@tagsTblStrings", SqlDbType.Structured).Value = tagsDt;
            cmdSearch.Parameters.Add("@freetext", SqlDbType.NVarChar, 500).Value = clsSearchInput.FreeTextSearch;
            cmdSearch.Parameters.Add("@maxResults", SqlDbType.Int).Value = clsSearchInput.maxResults;

            SqlDataAdapter da = new(cmdSearch);
            DataTable dt = new();
            da.Fill(dt);

            sqlConn.Close();

            return FillResultList(dt, clsSearchInput.ConnectionString, false);
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            //return null;
        }

        private static IEnumerable<ClsPublicSearchInfo> AutoCompleteSearch(ClsPublicSearchAutocomplete clsSearchInput)
        {
            SqlConnection sqlConn = new(clsSearchInput.ConnectionString);
            sqlConn.Open();

            // Configure the SqlCommand and SqlParameter.  
            SqlCommand cmdSearch = new("kk_aj_proc_autocompleteSearch", sqlConn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmdSearch.Parameters.Add("@searchText", SqlDbType.NVarChar).Value = clsSearchInput.SearchText;
            cmdSearch.Parameters.Add("@maxResults", SqlDbType.Int).Value = clsSearchInput.MaxResults;

            SqlDataAdapter da = new(cmdSearch);
            DataTable dt = new();
            da.Fill(dt);

            sqlConn.Close();

            return FillResultList(dt, clsSearchInput.ConnectionString, true);
        }

        private static List<ClsPublicSearchInfo> FillResultList(DataTable dt, string connectionString, bool minimumResult)
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

        private static List<faktainfo> GetFaktaInfo(int arrID, string ConnectionString)
        {
            SqlConnection sqlConn = new(ConnectionString);
            sqlConn.Open();

            List<faktainfo> resultList = new();

            // Configure the SqlCommand and SqlParameter.  
            SqlCommand cmdSearch = new("kk_aj_proc_getfaktabyarrid", sqlConn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmdSearch.Parameters.Add("@arrid", SqlDbType.Int).Value = arrID;

            SqlDataAdapter da = new(cmdSearch);
            DataTable dt = new();
            da.Fill(dt);

            sqlConn.Close();

            foreach (DataRow row in dt.Rows)
            {
                faktainfo fInfo = new();
                fInfo.Faktaid = (int)row["faktaid"];
                fInfo.Faktarubrik = row["Faktarubrik"].ToString();
                fInfo.FaktaTypID = (int)row["faktatypid"];
                fInfo.FaktaValue = row["faktavalue"].ToString();

                resultList.Add(fInfo);
            }

            return resultList;
        }

        private static List<utovareInfo> GetUtovareInfo(int UtovarID, string ConnectionString)
        {
            SqlConnection sqlConn = new(ConnectionString);
            sqlConn.Open();

            List<utovareInfo> resultList = new();

            // Configure the SqlCommand and SqlParameter.  
            SqlCommand cmdSearch = new("kk_aj_proc_getUtovareById", sqlConn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmdSearch.Parameters.Add("@UtovarID", SqlDbType.Int).Value = UtovarID;

            SqlDataAdapter da = new(cmdSearch);
            DataTable dt = new();
            da.Fill(dt);

            sqlConn.Close();

            foreach (DataRow row in dt.Rows)
            {
                utovareInfo uInfo = new();
                uInfo.UtovarID = (int)row["UtovarID"];
                uInfo.Organisation = row["Organisation"].ToString();
                uInfo.Fornamn = row["Fornamn"].ToString();
                uInfo.Efternamn = row["Efternamn"].ToString();
                uInfo.Telefon = row["Telefonnummer"].ToString();
                uInfo.Adress = row["Adress"].ToString();
                uInfo.Postnr = row["Postnr"].ToString();
                uInfo.Ort = row["Ort"].ToString();
                uInfo.Epost = row["Epost"].ToString();
                uInfo.Kommun = row["Kommun"].ToString();
                uInfo.Weburl = row["Hemsida"].ToString();

                resultList.Add(uInfo);
            }

            return resultList;
        }

        private static List<mediaInfo> GetMediaInfo(int arrID, string ConnectionString)
        {
            SqlConnection sqlConn = new(ConnectionString);
            sqlConn.Open();

            List<mediaInfo> resultList = new();

            // Configure the SqlCommand and SqlParameter.  
            SqlCommand cmdSearch = new("kk_aj_proc_GetMediaByArrid", sqlConn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmdSearch.Parameters.Add("@ArrID", SqlDbType.Int).Value = arrID;

            SqlDataAdapter da = new(cmdSearch);
            DataTable dt = new();
            da.Fill(dt);

            sqlConn.Close();

            foreach (DataRow row in dt.Rows)
            {
                mediaInfo mInfo = new();
                mInfo.MediaID = (int)row["mediaID"];
                mInfo.MediaUrl = row["mediaUrl"].ToString();
                mInfo.MediaFilename = row["mediaFileName"].ToString();
                mInfo.MediaSize = row["mediaSize"].ToString();
                mInfo.MediaAlt = row["mediaAlt"].ToString();
                mInfo.MediaFoto = row["mediaFoto"].ToString();
                mInfo.MediaTyp = row["mediatyp"].ToString();
                mInfo.MediaVald = row["mediaVald"].ToString();
                mInfo.mediaTitle = row["mediaTitle"].ToString();
                mInfo.mediaBeskrivning = row["mediaBeskrivning"].ToString();
                mInfo.mediaLink = row["mediaLink"].ToString();
                mInfo.sortering = row["sortering"].ToString();

                resultList.Add(mInfo);
            }

            return resultList;
        }
    }
}
