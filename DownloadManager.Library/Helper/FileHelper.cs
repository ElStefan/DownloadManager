using DownloadManager.Library.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DownloadManager.Library.Helper
{
    public static class FileHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileHelper));

        public static byte[] GetBinaries(int id)
        {
            var items = DatabaseHelper.GetAll();
            var item = items.FirstOrDefault(o => o.Id == id);
            if (item == null)
            {
                Log.Debug("Install - Cannot find appinfo in database");
                return null;
            }
            // get local if available and from today and return
            var localPath = Environment.CurrentDirectory + "\\cache\\" + item.Id + ".exe";
            if (!Directory.Exists(Environment.CurrentDirectory + "\\cache"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\cache");
            }
            if (File.Exists(localPath) && File.GetCreationTime(localPath).Date >= DateTime.Now.Date.AddMonths(-1))
            {
                return File.ReadAllBytes(localPath);
            }

            var bytes = LinkHelper.Download(item.Link);
            // store app local
            File.WriteAllBytes(localPath, bytes);

            return bytes;
        }

        public static RequestResult RemoveCachedFile(int id)//
        {
            var result = new RequestResult { Success = false };
            var localPath = Environment.CurrentDirectory + "\\cache\\" + id + ".exe";
            if (!File.Exists(localPath))
            {
                result.Message = "Cannot clear, file not found";
                return result;
            }
            File.Delete(localPath);
            result.Success = true;
            result.Message = "Cached file removed";
            return result;
        }

        public static RequestResult ClearAllCachedFiles()
        {
            var result = new RequestResult { Success = false };
            var items = DatabaseHelper.GetAll();
            if (items == null)
            {
                result.Message = "Cannot get items from database";
                return result;
            }
            foreach (var item in items)
            {
                var localPath = Environment.CurrentDirectory + "\\cache\\" + item.Id + ".exe";
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }
            }
            result.Success = true;
            return result;
        }

        public static RequestResult CreateCachedFiles(List<int> ids)
        {
            var result = new RequestResult { Success = false };
            var messages = new StringBuilder();
            var failCount = 0;
            foreach (var id in ids)
            {
                var tempResult = CreateCachedFile(id);
                if (!tempResult.Success)
                {
                    messages.AppendLine(tempResult.Message);
                    failCount++;
                }
            }
            if (failCount == ids.Count)
            {
                result.Message = "Error - no items were cached";
                return result;
            }
            result.Success = true;
            result.Message = messages.ToString();
            return result;
        }

        public static RequestResult CreateCachedFile(int id)
        {
            var result = new RequestResult();
            result.Success = false;
            var bytes = GetBinaries(id);
            if (bytes == null)
            {
                Log.ErrorFormat("CreateCachedFile - Cannot cache item id: {0}", id);
                result.Message = "Cannot cache item";
                return result;
            }
            result.Success = true;
            return result;
        }
    }
}