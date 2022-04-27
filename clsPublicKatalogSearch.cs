using KulturkatalogenDomain.Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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

            SqlDataAdapter da = new(cmdSearch);
            DataTable dt = new();
            da.Fill(dt);

            sqlConn.Close();

            return FillResultList(dt, clsSearchInput.ConnectionString);
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            //return null;
        }

        private static IEnumerable<PublicSearchReturnJsonInfo> AutoCompleteSearch(ClsPublicSearchAutocomplete clsSearchInput)
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

            return FillResultList(dt, clsSearchInput.ConnectionString);
        }

        private static List<PublicSearchReturnJsonInfo> FillResultList(DataTable dt, string connectionString)
        {
            List<PublicSearchReturnJsonInfo> lstReturnValue = new();

            foreach (DataRow row in dt.Rows)
            {
                //Skapa ett objekt per rad i svaret, och fyll det med innehållet i varje kolumn
                PublicSearchReturnJsonInfo rowObject = new();

                rowObject.ArkivStatus = row["ArkivStatus"].ToString();
                rowObject.ansokningtypid = row["ArrangemangstypID"].ToString();
                rowObject.ansokningid = (int)row["ArrID"];
                rowObject.ansokningdate = row["Datum"].ToString();
                rowObject.ansokningMediaImage.MediaUrl = row["ImageUrl"].ToString();
                rowObject.ansokningkonstform = row["konstform"].ToString();
                rowObject.ansokningKonstform2 = row["konstform2"].ToString();
                rowObject.ansokningKonstform3 = row["konstform3"].ToString();
                rowObject.ansokningutovare = row["Organisation"].ToString();
                rowObject.PeriodSlut = (row["periodslut"] == DBNull.Value ? null : (DateTime?)row["periodslut"]).ToString();
                rowObject.PeriodStart = (row["periodstart"] == DBNull.Value ? null : (DateTime?)row["periodstart"]).ToString();
                rowObject.ansokningpublicerad = row["Publicerad"].ToString();
                rowObject.ansokningtitle = row["Rubrik"].ToString();
                //rowObject.Startyear = row["startyear"].ToString();
                //rowObject.Stoppyear = row["stoppyear"].ToString();
                rowObject.ansokningsubtitle = row["Underrubrik"].ToString();
                rowObject.ansokningUtovarid = (int)row["UtovarID"];
                rowObject.ansokningFaktalist = new List<faktainfo>(); // GetFaktaInfo((int)row["ArrID"], connectionString);
                rowObject.ansokningUtovardata = new utovareInfo(); // GetUtovareInfo((int)row["UtovarID"], connectionString);
                rowObject.ansokningMedialist = new List<mediaInfo>(); // GetMediaInfo((int)row["ArrID"], connectionString);
                //rowObject.ListFilterFaktaInfo = new List<filterfaktaInfo>();

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

        private static utovareInfo GetUtovareInfo(int UtovarID, string ConnectionString)
        {
            SqlConnection sqlConn = new(ConnectionString);
            sqlConn.Open();

            //List<utovareInfo> resultList = new();

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

            //foreach (DataRow row in dt.Rows)
            //{
                utovareInfo uInfo = new();
                uInfo.UtovarID = (int)dt.Rows[0]["UtovarID"];
                uInfo.Organisation = dt.Rows[0]["Organisation"].ToString();
                uInfo.Fornamn = dt.Rows[0]["Fornamn"].ToString();
                uInfo.Efternamn = dt.Rows[0]["Efternamn"].ToString();
                uInfo.Telefon = dt.Rows[0]["Telefonnummer"].ToString();
                uInfo.Adress = dt.Rows[0]["Adress"].ToString();
                uInfo.Postnr = dt.Rows[0]["Postnr"].ToString();
                uInfo.Ort = dt.Rows[0]["Ort"].ToString();
                uInfo.Epost= dt.Rows[0]["Epost"].ToString();
                uInfo.Kommun= dt.Rows[0]["Kommun"].ToString();
                uInfo.Weburl= dt.Rows[0]["Hemsida"].ToString();


            return uInfo;
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
                mInfo.MediaTyp= row["mediatyp"].ToString();
                mInfo.MediaVald= row["mediaVald"].ToString();
                mInfo.mediaTitle= row["mediaTitle"].ToString();
                mInfo.mediaBeskrivning= row["mediaBeskrivning"].ToString();
                mInfo.mediaLink = row["mediaLink"].ToString();
                mInfo.sortering = row["sortering"].ToString();

                resultList.Add(mInfo);
            }

            return resultList;
        }
    }
}
