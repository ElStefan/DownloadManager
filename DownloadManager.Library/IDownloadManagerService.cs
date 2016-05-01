using DownloadManager.Library.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DownloadManager.Library
{
    [ServiceContract]
    public interface IDownloadManagerService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        RequestResult<List<AppInfo>> GetApps();

        [OperationContract]
        RequestResult Update(AppInfo app);

        [OperationContract]
        RequestResult Add(AppInfo app);

        [OperationContract]
        RequestResult Remove(int id);

        [OperationContract]
        RequestResult<byte[]> GetBinaries(int id);

        [OperationContract]
        RequestResult RemoveCachedFile(int id);

        [OperationContract]
        RequestResult ClearAllCachedFiles();

        [OperationContract]
        RequestResult CreateCachedFile(int id);

        [OperationContract]
        RequestResult CreateCachedFiles(List<int> ids);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "DownloadManager.Library.ContractType".
    [DataContract]
    public class CompositeType
    {
        private bool boolValue = true;
        private string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}