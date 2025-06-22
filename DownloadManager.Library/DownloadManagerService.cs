using DownloadManager.Library.Helper;
using DownloadManager.Library.Models;
using Serilog;
using System;
using System.Collections.Generic;

namespace DownloadManager.Library
{
    public class DownloadManagerService : IDownloadManagerService
    {

        private static readonly ILogger Log = Log.ForContext(typeof(DownloadManagerService));
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public RequestResult<List<AppInfo>> GetApps()
        {
            try
            {
                var result = new RequestResult<List<AppInfo>>();
                var data = DatabaseHelper.GetAll();
                if (data == null)
                {
                    result.Message = "Unknown error";
                    result.Success = false;
                    return result;
                }
                result.Data = data;
                result.Success = true;
                return result;
            }
            catch (Exception exception)
            {
                Log.Error("GetApps", exception);
                return new RequestResult<List<AppInfo>>() { Message = "Service error", Success = false };
            }
        }

        public RequestResult Update(AppInfo app)
        {
            try
            {
                return DatabaseHelper.Update(app, false);
            }
            catch (Exception exception)
            {
                Log.Error("Update", exception);
                return new RequestResult() { Message = "Service error", Success = false };
            }
        }

        public RequestResult Add(AppInfo app)
        {
            try
            {
                return DatabaseHelper.Insert(app);
            }
            catch (Exception exception)
            {
                Log.Error("Add", exception);
                return new RequestResult() { Message = "Service error", Success = false };
            }
        }

        public RequestResult Remove(int id)
        {
            try
            {
                return DatabaseHelper.Remove(id);
            }
            catch (Exception exception)
            {
                Log.Error("Remove", exception);
                return new RequestResult() { Message = "Service error", Success = false };
            }
        }

        public RequestResult<byte[]> GetBinaries(int id)
        {
            try
            {
                var result = new RequestResult<byte[]>();
                var data = FileHelper.GetBinaries(id);
                result.Data = data;
                result.Success = true;
                if (data == null)
                {
                    result.Success = false;
                    result.Message = "Service error";
                    return result;
                }
                if (data.Length == 0)
                {
                    result.Success = false;
                    result.Message = "Service error";
                }
                return result;
            }
            catch (Exception exception)
            {
                Log.Error("GetBinaries", exception);
                return new RequestResult<byte[]>() { Message = "Service error", Success = false };
            }
        }

        public RequestResult RemoveCachedFile(int id)
        {
            try
            {
                return FileHelper.RemoveCachedFile(id);
            }
            catch (Exception exception)
            {
                Log.Error("RemoveCachedFile", exception);
                return new RequestResult() { Message = "Service error", Success = false };
            }
        }

        public RequestResult ClearAllCachedFiles()
        {
            try
            {
                return FileHelper.ClearAllCachedFiles();
            }
            catch (Exception exception)
            {
                Log.Error("ClearAllCachedFiles", exception);
                return new RequestResult() { Message = "Service error", Success = false };
            }
        }

        public RequestResult CreateCachedFile(int id)
        {
            try
            {
                return FileHelper.CreateCachedFile(id);
            }
            catch (Exception exception)
            {
                Log.Error("CreateCachedFile", exception);
                return new RequestResult() { Message = "Service error", Success = false };
            }
        }

        public RequestResult CreateCachedFiles(List<int> ids)
        {
            try
            {
                return FileHelper.CreateCachedFiles(ids);
            }
            catch (Exception exception)
            {
                Log.Error("CreateCachedFiles", exception);
                return new RequestResult() { Message = "Service error", Success = false };
            }
        }
    }
}