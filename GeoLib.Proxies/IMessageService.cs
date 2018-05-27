using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GeoLib.Client.Contracts
{
    [ServiceContract(Namespace = "http://www.pluralsight.com/test")]
    public interface IMessageService
    {
        [OperationContract]
        void ShowMsg(string message);
    }
}
