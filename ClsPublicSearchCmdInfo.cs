using System.Collections.Generic;

namespace kk_lib_getFromDB
{
    public class ClsPublicSearchCmdInfo
    {
        private List<string> _Tags;
        private List<int> _KonstartIDs;


        public int ArrTypID { get; set; }
        public string CmdTyp { get; set; }
        public string FreeTextSearch { get; set; }
        public List<int> AgeSpans { get; set; }
        public string IsPublic { get; set; }
        public string ConnectionString { get; set; }
        public int maxResults { get; set; }

        public List<string> Tags
        {
            get
            {
                return _Tags;
            }
            set
            {
                value.RemoveAll(x => string.IsNullOrEmpty(x));
                _Tags = value;
            }
        }

        public List<int> KonstartIDs
        {
            get
            {
                return _KonstartIDs;
            }
            set
            {
                value.RemoveAll(x => x == 0);
                _KonstartIDs = value;
            }
        }

    }
}
