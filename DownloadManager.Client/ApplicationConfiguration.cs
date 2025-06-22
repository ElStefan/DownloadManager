using System;
using System.Windows.Forms;

namespace DownloadManager.Client
{
    internal static class ApplicationConfiguration
    {
        public static void Initialize()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
