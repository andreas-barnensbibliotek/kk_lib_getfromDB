using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace kk_lib_getFromDB
{

    public class ClsPublicSearchInfo
    {
        //private int? _ArkivStatus;

        public int? ArkivStatus { get; set; }
        public int? ArrangemangstypID { get; set; }
        public int ArrID { get; set; }
        public DateTime? Datum { get; set; }
        public string ImageUrl { get; set; }
        public string Konstform { get; set; }
        public int? Konstform2 { get; set; }
        public int? Konstform3 { get; set; }
        public string Organisation { get; set; }
        public DateTime? periodslut { get; set; }
        public DateTime? periodstart { get; set; }
        public string Publicerad { get; set; }
        public string Rubrik { get; set; }
        public string Startyear { get; set; }
        public string Stoppyear { get; set; }
        public string Underrubrik { get; set; }
        public int? UtovarID { get; set; }
    }

    public class ClsPublicSearchCmdInfo
    {
        public int ArrTypID { get; set; }
        public string CmdTyp { get; set; }
        public string FreeTextSearch { get; set; }
        public int[] KonstartIDs { get; set; }
        public int[,] AgeSpans { get; set; }
        public string[] Tags { get; set; }
        public string IsPublic { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ClsPublicSearchAutocomplete
    {
        public string SearchText { get; set; }
        public int MaxResults { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ClsQuery
    {
        /// <summary>
        /// Gör en sökning i databasen
        /// Exempel: 
        /// ClsPublicSearchCmdInfo qryParams = new()
        /// {
        ///         ArrTypID = 0,
        ///         CmdTyp = "",
        ///         KonstartIDs = new int[] { 2, 3 },
        ///         AgeSpans = new int[,] { { 0, 2 }, { 10, 11 } },
        ///         Tags = new string[] { "foo","bar" },
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
            ageDt.Columns.Add("from");
            ageDt.Columns.Add("to");
            for (int i = 0; i < clsSearchInput.AgeSpans.GetLength(0); i++)
            {
                DataRow dr;
                dr = ageDt.NewRow();
                dr["from"] = clsSearchInput.AgeSpans[i, 0];
                dr["to"] = clsSearchInput.AgeSpans[i, 1];
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
            SqlCommand cmdSearch = new("kk_aj_proc_Search", sqlConn)
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

            return FillResultList(dt);
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

            return FillResultList(dt);
        }

        private static List<ClsPublicSearchInfo> FillResultList(DataTable dt)
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
                rowObject.Underrubrik = row["Underrubrik"].ToString();
                rowObject.UtovarID = (int?)row["UtovarID"];

                lstReturnValue.Add(rowObject);
            }

            return lstReturnValue;
        }
    }
}
