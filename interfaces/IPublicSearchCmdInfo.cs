namespace KulturkatalogenDomain.Info
{
    public interface IPublicSearchCmdInfo
    {
        int arrTypID { get; set; }
        string cmdTyp { get; set; }
        string freeTextSearch { get; set; }
        int konstartID { get; set; }
        int startYear { get; set; }
        int stoppYear { get; set; }
    }
}