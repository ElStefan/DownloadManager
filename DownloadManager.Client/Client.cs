using BrightIdeasSoftware;
using DownloadManager.Client.ServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadManager.Client
{
    public partial class Client : Form
    {
        private string _server;
        private int? _selectedId;

        public Client()
        {
            InitializeComponent();
            this._server = "net.tcp://server10.lampertnet:8090/DMS";
            if (!String.IsNullOrEmpty(this._server))
            {
                Task.Run(() => this.UpdateList());
            }
        }

        private async Task UpdateList()
        {
            var result = await this.GetAppsAsync();
            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }
            this.objectListViewItems.SetObjects(result.Data);
        }

        private void objectListViewItems_SelectionChanged(object sender, EventArgs e)
        {
            var item = this.objectListViewItems.SelectedObject as AppInfo;
            if (item == null)
            {
                return;
            }
            this.textBoxName.Text = item.Name;
            this.textBoxLink.Text = item.Link;
            this.textBoxWiki.Text = item.WikiLink;
            this._selectedId = item.Id;
        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (this._selectedId != null)
            {
                var item = new AppInfo() { Id = this._selectedId.Value, Name = this.textBoxName.Text, Link = this.textBoxLink.Text, WikiLink = this.textBoxWiki.Text };
                var result = await this.UpdateAsync(item);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message);
                    return;
                }
            }
            await this.UpdateList();
        }

        private async void buttonAdd_Click(object sender, EventArgs e)
        {
            var item = new AppInfo() { Name = this.textBoxName.Text, Link = this.textBoxLink.Text, WikiLink = this.textBoxWiki.Text };

            var result = await this.AddAsync(item);
            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }
            await this.UpdateList();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => this.UpdateList());
        }

        private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mboxResult = MessageBox.Show("Do you really want to remove this app?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (mboxResult != DialogResult.Yes)
            {
                return;
            }
            if (this._selectedId != null)
            {
                var result = await this.RemoveAppAsync(this._selectedId.Value);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message);
                    return;
                }
                MessageBox.Show("App successfully removed");
            }

            await this.UpdateList();
        }

        private async void installToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var items = this.objectListViewItems.SelectedObjects.Cast<AppInfo>();
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                var localPath = Environment.CurrentDirectory + "\\temp\\" + item.Id + ".exe";
                if (!Directory.Exists(Environment.CurrentDirectory + "\\temp"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\temp");
                }
                var result = await this.GetBinariesAsync(item.Id); // extend to "GetManyBinaries(List<int> ids)
                if (!result.Success)
                {
                    MessageBox.Show(result.Message);
                    return;
                }
                File.WriteAllBytes(localPath, result.Data);
                this.InstallApp(localPath, item.Name);
            }
        }

        private void InstallApp(string localPath, string name)
        {
            try
            {
                using (var process = Process.Start(new ProcessStartInfo(localPath)))
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        MessageBox.Show(String.Format("{0} successfully installed", name));
                        return;
                    }
                    MessageBox.Show(String.Format("{0} installation failed", name));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
            finally
            {
                File.Delete(localPath);
            }
        }

        private async void clearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = await this.ClearAllCachedFilesAsync();
            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }
        }

        private async void removeSelectedCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._selectedId != null)
            {
                var result = await this.RemoveCachedFileAsync(this._selectedId.Value);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message);
                    return;
                }
            }
        }

        private async void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var items = this.objectListViewItems.SelectedObjects.Cast<AppInfo>();
            if (items == null)
            {
                return;
            }
            var result = await this.CreateCachedFilesAsync(items.Select(o => o.Id).ToList());
            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }
        }

        private Task<RequestResultOfArrayOfAppInfowtz7jBEH> GetAppsAsync()
        {
            return Task.Run(() =>
            {
                using (var client = new DownloadManagerServiceClient("DownloadManagerServiceEndpoint", this._server))
                {
                    return client.GetApps();
                }
            });
        }

        private Task<RequestResult> ClearAllCachedFilesAsync()
        {
            return Task.Run(() =>
            {
                using (var client = new DownloadManagerServiceClient("DownloadManagerServiceEndpoint", this._server))
                {
                    return client.ClearAllCachedFiles();
                }
            });
        }

        private Task<RequestResult> RemoveCachedFileAsync(int id)
        {
            return Task.Run(() =>
            {
                using (var client = new DownloadManagerServiceClient("DownloadManagerServiceEndpoint", this._server))
                {
                    return client.RemoveCachedFile(id);
                }
            });
        }

        private Task<RequestResult> CreateCachedFilesAsync(List<int> ids)
        {
            return Task.Run(() =>
            {
                using (var client = new DownloadManagerServiceClient("DownloadManagerServiceEndpoint", this._server))
                {
                    return client.CreateCachedFiles(ids);
                }
            });
        }

        private Task<RequestResult> UpdateAsync(AppInfo item)
        {
            return Task.Run(() =>
            {
                using (var client = new DownloadManagerServiceClient("DownloadManagerServiceEndpoint", this._server))
                {
                    return client.Update(item);
                }
            });
        }

        private Task<RequestResult> AddAsync(AppInfo item)
        {
            return Task.Run(() =>
            {
                using (var client = new DownloadManagerServiceClient("DownloadManagerServiceEndpoint", this._server))
                {
                    return client.Add(item);
                }
            });
        }

        private Task<RequestResult> RemoveAppAsync(int id)
        {
            return Task.Run(() =>
            {
                using (var client = new DownloadManagerServiceClient("DownloadManagerServiceEndpoint", this._server))
                {
                    return client.Remove(id);
                }
            });
        }

        private async Task<RequestResultOfbase64Binary> GetBinariesAsync(int id)
        {
            return await Task.Run(() =>
            {
                using (var client = new DownloadManagerServiceClient("DownloadManagerServiceEndpoint", this._server))
                {
                    return client.GetBinaries(id);
                }
            });
        }

        private void objectListViewItems_FormatRow(object sender, FormatRowEventArgs e)
        {
            var item = e.Model as AppInfo;
            if (item == null)
            {
                return;
            }
            if (!item.LinkValid)
            {
                e.Item.BackColor = Color.Salmon;
                return;
            }
            e.Item.BackColor = Color.White;
        }
    }
}