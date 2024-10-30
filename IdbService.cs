using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace simpleDashBoard
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IdbService" in both code and config file together.
    [ServiceContract]
    public interface IdbService
    {
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "getScreenData?year={year}")]
        screenData[] getScreenData(string year);
    }
}
