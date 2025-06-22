using DownloadManager.Library.Models;
using System.Collections.Generic;

namespace DownloadManager.Library
{
    
    public interface IDownloadManagerService
    {
        
        string GetData(int value);

        
        CompositeType GetDataUsingDataContract(CompositeType composite);

        
        RequestResult<List<AppInfo>> GetApps();

        
        RequestResult Update(AppInfo app);

        
        RequestResult Add(AppInfo app);

        
        RequestResult Remove(int id);

        
        RequestResult<byte[]> GetBinaries(int id);

        
        RequestResult RemoveCachedFile(int id);

        
        RequestResult ClearAllCachedFiles();

        
        RequestResult CreateCachedFile(int id);

        
        RequestResult CreateCachedFiles(List<int> ids);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "DownloadManager.Library.ContractType".
    
    public class CompositeType
    {
        private bool boolValue = true;
        private string stringValue = "Hello ";

        
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}