using System.Collections.Generic;

namespace kk_lib_getFromDB
{
    public class PublicSearchReturnJsonInfo
    {

        private int _ansokningid;
        private string _ansokningdate;
        private string _ansokningcontentid;
        private string _ansokningtitle;
        private string _ansokningsubtitle;
        private string _ansokningpublicerad;
        private string _ansokninglast;
        private string _ansokningstatus;
        private string _ansokningtypid;
        private string _ansokningkonstformid;
        private string _ansokningtyp;
        private string _ansokningkonstform;
        private string _ansokningutovare;
        private int _utovareid;
        private string _konstform2;
        private string _konstform3;
        private string _kontatkid;
        private string _kontaktFornamn;
        private string _kontaktEfternamn;
        private string _kontakttelefon;
        private string _kontaktEpost;
        private string _periodStart;
        private string _periodSlut;
        private string _arkivstatus;
        private string _innehall;
        private string _ansokningurl;
        private string _ansokningbilaga;
        private List<mediaInfo> _ansokningmovieclip;
        private List<faktainfo> _faktalist;
        private List<mediaInfo> _medialist;
        private string _ansokusername;
        private mediaInfo _ansokningmediaimage;
        private utovareInfo _utovardata;
        private string _agesspan;
        private int _startyear;
        private int _stoppyear;
        private filterfaktaInfo _filterfakta;
        public PublicSearchReturnJsonInfo()
        {
            _ansokningid = 0;
            _ansokningdate = "";
            _ansokningcontentid = "";
            _ansokningtitle = "";
            _ansokningsubtitle = "";
            _ansokningpublicerad = "Nej";
            _ansokninglast = "Nej";
            _ansokningstatus = "";
            _ansokningtypid = "0";
            _ansokningkonstformid = "0";
            _ansokningtyp = "";
            _ansokningkonstform = "";
            _ansokningutovare = "";
            _ansokningurl = "";
            _ansokningbilaga = "";
            _ansokningmediaimage = new mediaInfo();
            _ansokusername = "";
            _utovareid = 0;
            _utovardata = new utovareInfo();
            _konstform2 = "0";
            _konstform3 = "0";
            _kontatkid = "";
            _kontaktFornamn = "";
            _kontaktEfternamn = "";
            _kontakttelefon = "";
            _kontaktEpost = "";
            _periodStart = "";
            _periodSlut = "";
            _arkivstatus = "0";
        }
        public int ansokningid
        {
            get
            {
                return _ansokningid;
            }
            set
            {
                _ansokningid = value;
            }
        }

        public string ansokningdate
        {
            get
            {
                return _ansokningdate;
            }
            set
            {
                _ansokningdate = value;
            }
        }

        public string ansokningcontentid
        {
            get
            {
                return _ansokningcontentid;
            }
            set
            {
                _ansokningcontentid = value;
            }
        }
        public string ansokningtitle
        {
            get
            {
                return _ansokningtitle;
            }
            set
            {
                _ansokningtitle = value;
            }
        }

        public string ansokningsubtitle
        {
            get
            {
                return _ansokningsubtitle;
            }
            set
            {
                _ansokningsubtitle = value;
            }
        }

        public string ansokningContent
        {
            get
            {
                return _innehall;
            }
            set
            {
                _innehall = value;
            }
        }
        public string ansokningutovare
        {
            get
            {
                return _ansokningutovare;
            }
            set
            {
                _ansokningutovare = value;
            }
        }

        public string ansokningurl
        {
            get
            {
                return _ansokningurl;
            }
            set
            {
                _ansokningurl = value;
            }
        }

        public string ansokningbilaga
        {
            get
            {
                return _ansokningbilaga;
            }
            set
            {
                _ansokningbilaga = value;
            }
        }
        public string ansokningpublicerad
        {
            get
            {
                return _ansokningpublicerad;
            }
            set
            {
                _ansokningpublicerad = value;
            }
        }

        public string ansokninglast
        {
            get
            {
                return _ansokninglast;
            }
            set
            {
                _ansokninglast = value;
            }
        }

        public string ansokningstatus
        {
            get
            {
                return _ansokningstatus;
            }
            set
            {
                _ansokningstatus = value;
            }
        }

        public string ansokningtypid
        {
            get
            {
                return _ansokningtypid;
            }
            set
            {
                _ansokningtypid = value;
            }
        }

        public string ansokningkonstformid
        {
            get
            {
                return _ansokningkonstformid;
            }
            set
            {
                _ansokningkonstformid = value;
            }
        }

        public string ansokningtyp
        {
            get
            {
                return _ansokningtyp;
            }
            set
            {
                _ansokningtyp = value;
            }
        }

        public string ansokningkonstform
        {
            get
            {
                return _ansokningkonstform;
            }
            set
            {
                _ansokningkonstform = value;
            }
        }

        public List<mediaInfo> ansokningmovieclip
        {
            get
            {
                return _ansokningmovieclip;
            }
            set
            {
                _ansokningmovieclip = value;
            }
        }

        public List<faktainfo> ansokningFaktalist
        {
            get
            {
                return _faktalist;
            }
            set
            {
                _faktalist = value;
            }
        }

        public List<mediaInfo> ansokningMedialist
        {
            get
            {
                return _medialist;
            }
            set
            {
                _medialist = value;
            }
        }

        public string ansokningUsername
        {
            get
            {
                return _ansokusername;
            }
            set
            {
                _ansokusername = value;
            }
        }

        public mediaInfo ansokningMediaImage
        {
            get
            {
                return _ansokningmediaimage;
            }
            set
            {
                _ansokningmediaimage = value;
            }
        }

        public int ansokningUtovarid
        {
            get
            {
                return _utovareid;
            }
            set
            {
                _utovareid = value;
            }
        }


        public utovareInfo ansokningUtovardata
        {
            get
            {
                return _utovardata;
            }
            set
            {
                _utovardata = value;
            }
        }

        public string ansokningAgespan
        {
            get
            {
                return _agesspan;
            }
            set
            {
                _agesspan = value;
            }
        }

        public int ansokningStartyear
        {
            get
            {
                return _startyear;
            }
            set
            {
                _startyear = value;
                _agesspan = value + "-" + _stoppyear;
            }
        }

        public int ansokningStoppyear
        {
            get
            {
                return _stoppyear;
            }
            set
            {
                _stoppyear = value;
                _agesspan = _startyear + "-" + value;
            }
        }

        public filterfaktaInfo ansokningFilterfakta
        {
            get
            {
                return _filterfakta;
            }
            set
            {
                _filterfakta = value;
            }
        }
        public string ansokningKonstform2
        {
            get
            {
                return _konstform2;
            }
            set
            {
                _konstform2 = value;
            }
        }

        public string ansokningKonstform3
        {
            get
            {
                return _konstform3;
            }
            set
            {
                _konstform3 = value;
            }
        }
        public string ansokningKontaktId
        {
            get
            {
                return _kontatkid;
            }
            set
            {
                _kontatkid = value;
            }
        }

        public string ansokningKontaktfornamn
        {
            get
            {
                return _kontaktFornamn;
            }
            set
            {
                _kontaktFornamn = value;
            }
        }
        public string ansokningKontaktEfternamn
        {
            get
            {
                return _kontaktEfternamn;
            }
            set
            {
                _kontaktEfternamn = value;
            }
        }
        public string ansokningKontaktTelefon
        {
            get
            {
                return _kontakttelefon;
            }
            set
            {
                _kontakttelefon = value;
            }
        }
        public string ansokningKontaktEpost
        {
            get
            {
                return _kontaktEpost;
            }
            set
            {
                _kontaktEpost = value;
            }
        }


        public string PeriodStart
        {
            get
            {
                return _periodStart;
            }
            set
            {
                _periodStart = value;
            }
        }

        public string PeriodSlut
        {
            get
            {
                return _periodSlut;
            }
            set
            {
                _periodSlut = value;
            }
        }

        public string ArkivStatus
        {
            get
            {
                return _arkivstatus;
            }
            set
            {
                _arkivstatus = value;
            }
        }
    }
}
