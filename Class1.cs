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
    }

    public class ClsQuery
    {
       
        public static IEnumerable<ClsPublicSearchInfo> DoSearch(ClsPublicSearchCmdInfo clsSearchInput)
        {
            IEnumerable<ClsPublicSearchInfo> searchResult;
            //searchResult = MainSearch(_connectionString, _arrangemangTypID, _arrKonstart, _arrAges, _arrTags, _isPublic, _freetext);
            searchResult = MainSearch(clsSearchInput);
            return searchResult;
        }


        private static IEnumerable<ClsPublicSearchInfo> MainSearch(ClsPublicSearchCmdInfo clsSearchInput)
        {
            //try
            //{

            string connString = "Server=localhost;Database=kulturkatalogenDB_SANDBOX;Trusted_Connection=True;";


            List<ClsPublicSearchInfo> lstReturnValue2 = new();

            SqlConnection sqlConn = new(connString);

            sqlConn.Open();

            //int[] arrKonstart = new int[] { 1, 2 };
            //int[,] arrAges = new int[,] { { 0, 2 }, { 10, 11 } };
            //string[] arrTags = new string[] { "foo" };

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
            SqlCommand cmdSearch = new("kk_aj_proc_Search_TVP", sqlConn)
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

            foreach (DataRow row in dt.Rows)
            {
                //Skapa ett objekt per rad i svaret, och fyll det med innehållet i varje kolumn
                ClsPublicSearchInfo qryResult = new();

                qryResult.ArkivStatus = (int?)row["ArkivStatus"];
                qryResult.ArrangemangstypID = (int?)row["ArrangemangstypID"];
                qryResult.ArrID = (int)row["ArrID"];
                qryResult.Datum = (DateTime?)row["Datum"];
                qryResult.ImageUrl = row["ImageUrl"].ToString();
                qryResult.Konstform = row["konstform"].ToString();
                qryResult.Konstform2 = (int?)row["konstform2"];
                qryResult.Konstform3 = (int?)row["konstform3"];
                qryResult.Organisation = row["Organisation"].ToString();
                qryResult.periodslut = (DateTime?)row["periodslut"];
                qryResult.periodstart = (DateTime?)row["periodstart"];
                qryResult.Publicerad = row["Publicerad"].ToString();
                qryResult.Rubrik = row["Rubrik"].ToString();
                qryResult.Startyear = row["startyear"].ToString();
                qryResult.Stoppyear = row["stoppyear"].ToString();
                qryResult.Underrubrik = row["Underrubrik"].ToString();
                qryResult.UtovarID = (int?)row["UtovarID"];

                lstReturnValue2.Add(qryResult);
            }

            //returnValue = dt.AsEnumerable();

            sqlConn.Close();

            return lstReturnValue2;
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            //return null;
        }

    }
}
