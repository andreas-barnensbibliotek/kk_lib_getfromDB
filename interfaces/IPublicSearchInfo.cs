using System;

namespace KulturkatalogenDomain.Info
{
    public interface IPublicSearchInfo
    {
        int? ArkivStatus { get; set; }
        int? ArrangemangstypID { get; set; }
        int ArrID { get; set; }
        DateTime? Datum { get; set; }
        string ImageUrl { get; set; }
        string konstform { get; set; }
        int? konstform2 { get; set; }
        int? konstform3 { get; set; }
        string Organisation { get; set; }
        DateTime? periodslut { get; set; }
        DateTime? periodstart { get; set; }
        string Publicerad { get; set; }
        string Rubrik { get; set; }
        string startyear { get; set; }
        string stoppyear { get; set; }
        string Underrubrik { get; set; }
        int? UtovarID { get; set; }
    }
}