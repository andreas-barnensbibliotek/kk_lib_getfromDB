using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace kk_lib_getFromDB
{

    public class ClsQuery
    {
        private string _connectionString;
        private int _arrangemangTypID;
        private int[] _arrKonstart;
        private int[,] _arrAges;
        private string[] _arrTags;
        private string _isPublic;
        private string _freetext;

        public String ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public int ArrangemangstypID
        {
            get { return _arrangemangTypID; }
            set { _arrangemangTypID = value; }
        }

        public int[] Konstarter
        {
            get { return _arrKonstart; }
            set { _arrKonstart = value; }
        }

        public int[,] AgeSpans
        {
            get { return _arrAges; }
            set { _arrAges = value; }
        }

        public string[] Tags
        {
            get { return _arrTags; }
            set { _arrTags = value; }
        }

        public bool Public
        {
            get { if (_isPublic == "ja") { return true; } else { return false; }; }
            set { if (value) { _isPublic = "ja"; } else { _isPublic = "nej"; }; }
        }

        public String FreeText
        {
            get { return _freetext; }
            set { _freetext = value; }
        }

        public IEnumerable<DataRow> DoSearch()
        {
            IEnumerable<DataRow> dt = MainSearch(_connectionString, _arrangemangTypID, _arrKonstart, _arrAges, _arrTags, _isPublic, _freetext);
            return dt;
        }

        //public void Print()
        //{
        //    Console.WriteLine("Class: {0} - Instance: {1}", ClassVariable, InstanceVariable);
        //}

        private static IEnumerable<DataRow> MainSearch(
            string connString,
            int arrangemangTypID,
            int[] arrKonstart,
            int[,] arrAges,
            string[] arrTags,
            string isPublic,
            string freetext
            )
        {
            try
            {
                IEnumerable<DataRow> returnValue;

                SqlConnection sqlConn = new(connString);

                sqlConn.Open();

                //int[] arrKonstart = new int[] { 1, 2 };
                //int[,] arrAges = new int[,] { { 0, 2 }, { 10, 11 } };
                //string[] arrTags = new string[] { "foo" };

                //Skapa och fyll datatables med de parametrar som kommer in i form av arrayer.
                //------------------------------------------------------------
                DataTable konstartDt = new();
                konstartDt.Columns.Add("konstart");
                foreach (int konstart in arrKonstart)
                {
                    DataRow dr;
                    dr = konstartDt.NewRow();
                    dr["konstart"] = konstart;
                    konstartDt.Rows.Add(dr);
                }

                DataTable ageDt = new();
                ageDt.Columns.Add("from");
                ageDt.Columns.Add("to");
                for (int i = 0; i < arrAges.GetLength(0); i++)
                {
                    DataRow dr;
                    dr = ageDt.NewRow();
                    dr["from"] = arrAges[i, 0];
                    dr["to"] = arrAges[i, 1];
                    ageDt.Rows.Add(dr);
                }

                DataTable tagsDt = new();
                tagsDt.Columns.Add("tag");
                foreach (string tag in arrTags)
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
                cmdSearch.Parameters.Add("@arrtypid", SqlDbType.Int).Value = arrangemangTypID;
                cmdSearch.Parameters.Add("@konstartTblIDs", SqlDbType.Structured).Value = konstartDt;
                cmdSearch.Parameters.Add("@ageLimitsTblInts", SqlDbType.Structured).Value = ageDt;
                cmdSearch.Parameters.Add("@pubyesno", SqlDbType.NVarChar, 3).Value = isPublic;
                cmdSearch.Parameters.Add("@tagsTblStrings", SqlDbType.Structured).Value = tagsDt;
                cmdSearch.Parameters.Add("@freetext", SqlDbType.NVarChar, 500).Value = freetext;

                SqlDataAdapter da = new(cmdSearch);
                DataTable dt = new();

                da.Fill(dt);

                returnValue = dt.AsEnumerable();

                sqlConn.Close();

                return (returnValue);
            }
            catch { }
            return null;
        }

    }
}
