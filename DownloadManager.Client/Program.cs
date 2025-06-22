using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DownloadManager.Client
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using var host = Host.CreateDefaultBuilder()
                .UseWindowsFormsLifetime()
                .UseSerilog((ctx, config) => config.WriteTo.Console())
                .ConfigureServices(services =>
                {
                    services.AddSingleton<MainForm>();
                })
                .Build();

            var form = host.Services.GetRequiredService<MainForm>();
            Application.Run(form);
        }
    }
}
