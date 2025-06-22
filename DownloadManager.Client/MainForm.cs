using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;
using DownloadManager.Server;
using Microsoft.Extensions.Configuration;

namespace DownloadManager.Client
{
    public partial class MainForm : Form
    {
        private readonly DownloadManager.DownloadManagerClient _client;

        public MainForm(IConfiguration configuration)
        {
            InitializeComponent();
            var url = configuration.GetValue<string>("GrpcUrl") ?? "http://localhost:5000";
            var channel = GrpcChannel.ForAddress(url);
            _client = new DownloadManager.DownloadManagerClient(channel);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            var apps = await _client.GetAppsAsync(new Empty());
            dataGridView1.DataSource = apps.Apps.ToList();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            var app = new AppInfo
            {
                Name = txtName.Text,
                Link = txtLink.Text,
                WikiLink = txtWiki.Text
            };
            await _client.AddAsync(app);
            await LoadData();
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is AppInfo app)
            {
                app.Name = txtName.Text;
                app.Link = txtLink.Text;
                app.WikiLink = txtWiki.Text;
                await _client.UpdateAsync(app);
                await LoadData();
            }
        }

        private async void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is AppInfo app)
            {
                await _client.RemoveAsync(new AppId { Id = app.Id });
                await LoadData();
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is AppInfo app)
            {
                var bin = await _client.GetBinariesAsync(new AppId { Id = app.Id });
                if (bin.Data.Length > 0)
                {
                    var path = $"{app.Name}.exe";
                    await System.IO.File.WriteAllBytesAsync(path, bin.Data.ToByteArray());
                    MessageBox.Show($"Saved to {path}");
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is AppInfo app)
            {
                txtName.Text = app.Name;
                txtLink.Text = app.Link;
                txtWiki.Text = app.WikiLink;
            }
        }
    }
}
