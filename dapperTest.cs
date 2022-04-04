using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace kk_lib_getFromDB
{
    public class dapperTest
    {
        public IEnumerable<ClsPublicSearchInfo> DapperTest()
            {
                const string connString = "Server=localhost;Database=kulturkatalogenDB_SANDBOX;Trusted_Connection=True;";

                var sql = "SELECT ArrID, Rubrik, Underrubrik, ImageUrl, Datum, ArrangemangstypID, Publicerad, startyear, stoppyear, minAge, maxAge, Organisation, UtovarID,konstform, KonstformID, konstform2, konstform3, periodstart, periodslut, ArkivStatus FROM kk_aj_view_arrSearch";
                //var products = new List<ClsPublicSearchInfo>();
                IEnumerable<ClsPublicSearchInfo> products;
                using (var connection = new SqlConnection(connString))
                {
                    connection.Open();
                    products = connection.Query<ClsPublicSearchInfo>(sql);
                }

                return products;
            }
    }
}
