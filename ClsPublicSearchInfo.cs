using KulturkatalogenDomain.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kk_lib_getFromDB
{
    public class ClsPublicSearchInfo
    {
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
        public List<faktainfo> ListFaktaInfo { get; set; }
        public utovareInfo ListUtovareInfo { get; set; }
        public mediaInfo Ansokningmediaimage { get; set; }
        public List<mediaInfo> ListMediaInfo { get; set; }
        public List<filterfaktaInfo> ListFilterFaktaInfo { get; set; }
    }
}
