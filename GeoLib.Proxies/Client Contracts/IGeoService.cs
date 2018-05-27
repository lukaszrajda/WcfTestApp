using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GeoLib.Client.Contracts
{
    //[ServiceContract(Namespace = "http://www.pluralsight.com/MiguelCastro/WcfEndToEnd")]
    [ServiceContract]
    public interface IGeoService
    {
        [OperationContract]
        ZipCodeData GetZipInfo(string zip);

        [OperationContract]
        IEnumerable<string> GetStates(bool primaryOnly);

        [OperationContract]
        IEnumerable<ZipCodeData> GetZipsByState(string state);

        [OperationContract]
        IEnumerable<ZipCodeData> GetZipsForRange(string zip, int range);
    }
}
