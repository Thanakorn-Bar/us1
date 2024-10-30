using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Configuration;

namespace simpleDashBoard
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "dbService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select dbService.svc or dbService.svc.cs at the Solution Explorer and start debugging.
    public class dbService : IdbService
    {
        public screenData[] getScreenData(string year)
        {
            List<screenData> retVal = new List<screenData>();
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["strconn"].ConnectionString);
            SqlCommand cmd = new SqlCommand(
              "SELECT S.[year], O.a_id, A.name AS area, O.o_id, O.o_name, S.total, S.screen, S.provide " +
              "FROM screen S " +
              "JOIN organize O ON  O.o_id = S.o_id " +
              "JOIN area A ON A.id = O.a_id  "
            , conn);
            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                screenData a = new screenData();
                a.year = rdr[0].ToString();
                a.a_id = rdr[1].ToString();
                a.area = rdr[2].ToString();
                a.o_id = rdr[3].ToString();
                a.o_name = rdr[4].ToString();
                a.total = rdr[5].ToString();
                a.screen = rdr[6].ToString();
                a.provide = rdr[7].ToString();

                retVal.Add(a);
            }

            rdr.Close();
            conn.Close();

            return retVal.ToArray();
        }
    }
}
