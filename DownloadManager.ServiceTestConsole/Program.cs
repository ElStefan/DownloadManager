using DownloadManager.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager.ServiceTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Controller.Start();
            Console.ReadLine();
            Controller.Stop();
        }
    }
}
