using DownloadManager.Library.Models;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DownloadManager.Library.Helper
{
    public class DatabaseHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DatabaseHelper));

        public static RequestResult Insert(AppInfo app)
        {
            var result = new RequestResult { Success = false };
            if (app == null)
            {
                Log.Error("Insert - Invalid item");
                result.Message = "Invalid item";
                return result;
            }
            var items = GetAll();
            var item = items.FirstOrDefault(o => o.Name == app.Name && o.Link == app.Link);
            if (item != null)
            {
                Log.Error("Insert - Item already exists");
                result.Message = "App already exists";
                return result;
            }
            if (String.IsNullOrEmpty(app.Name))
            {
                Log.Error("Insert - Invalid app name");
                result.Message = "Invalid app name";
                return result;
            }
            if (String.IsNullOrEmpty(app.Link))
            {
                Log.Error("Insert - Invalid app link");
                result.Message = "Invalid app link";
                return result;
            }
            if (!LinkHelper.ValidateApplication(app.Link))
            {
                result.Message = "Invalid app link";
                return result;
            }
            app.LinkValid = true;
            if (!String.IsNullOrEmpty(app.WikiLink))
            {
                if (!LinkHelper.ValidateWikipedia(app.WikiLink))
                {
                    result.Message = "Invalid wikipedia link";
                    return result;
                }
                app.WikiVersion = LinkHelper.GetNewestVersion(app.WikiLink);
            }
            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO appinfo (AppName,AppLink,WikiEntry,AppLinkValid) VALUES (@AppName,@AppLink,@WikiEntry,@AppLinkValid);";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        var linkValid = 0;
                        if (app.LinkValid)
                        {
                            linkValid = 1;
                        }
                        command.Parameters.AddWithValue("AppName", app.Name);
                        command.Parameters.AddWithValue("AppLink", app.Link);
                        command.Parameters.AddWithValue("WikiEntry", app.WikiLink);
                        command.Parameters.AddWithValue("AppLinkValid", linkValid);
                        var affected = command.ExecuteNonQuery();
                        if (affected == 1)
                        {
                            result.Success = true;
                            return result;
                        }

                        result.Message = "Insertion failed";
                        return result;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(nameof(Insert), exception);
                result.Message = "Exception - See log for details.";
                return result;
            }
        }

        public static RequestResult Update(AppInfo app, bool updateOnErrors)
        {
            var result = new RequestResult { Success = false };
            if (app == null)
            {
                Log.Error("Update - Invalid item");
                result.Message = "Invalid item";
                return result;
            }

            var items = GetAll();
            var item = items.FirstOrDefault(o => o.Id == app.Id);
            if (item == null)
            {
                result.Message = "App does not exist";
                return result;
            }

            if (!updateOnErrors)
            {
                if (String.IsNullOrEmpty(app.Name))
                {
                    Log.Error("Insert - Invalid app name");
                    result.Message = "Invalid app name";
                    return result;
                }
                if (String.IsNullOrEmpty(app.Link))
                {
                    Log.Error("Insert - Invalid app link");
                    result.Message = "Invalid app link";
                    return result;
                }
                if (!LinkHelper.ValidateApplication(app.Link))
                {
                    result.Message = "Invalid app link";
                    return result;
                }
                app.LinkValid = true;
                if (!String.IsNullOrEmpty(app.WikiLink))
                {
                    if (!LinkHelper.ValidateWikipedia(app.WikiLink))
                    {
                        result.Message = "Invalid wikipedia link";
                        return result;
                    }
                    app.WikiVersion = LinkHelper.GetNewestVersion(app.WikiLink);
                }
            }
            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    connection.Open();
                    var query = "UPDATE appinfo SET AppName=@AppName, AppLink=@AppLink, LastUpdate=@LastUpdate, WikiEntry=@WikiEntry, WikiVersion=@WikiVersion, AppLinkValid=@AppLinkValid WHERE Id=@Id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        var linkValid = 0;
                        if (app.LinkValid)
                        {
                            linkValid = 1;
                        }
                        command.Parameters.AddWithValue("Id", app.Id);
                        command.Parameters.AddWithValue("AppLink", app.Link);
                        command.Parameters.AddWithValue("AppLinkValid", linkValid);
                        command.Parameters.AddWithValue("AppName", app.Name);
                        command.Parameters.AddWithValue("LastUpdate", DateTime.Now);
                        command.Parameters.AddWithValue("WikiEntry", app.WikiLink);
                        command.Parameters.AddWithValue("WikiVersion", app.WikiVersion);
                        var affected = command.ExecuteNonQuery();
                        if (affected == 1)
                        {
                            result.Success = true;
                            return result;
                        }

                        result.Message = "Update failed";
                        return result;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(nameof(Update), exception);
                result.Message = "Exception - See log for details.";
                return result;
            }
        }

        public static List<AppInfo> GetAll()
        {
            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM appinfo";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        var reader = command.ExecuteReader();
                        var items = new List<AppInfo>();
                        while (reader.Read())
                        {
                            var item = new AppInfo
                            {
                                Id = reader.GetInt32("Id"),
                                LastUpdate = reader["LastUpdate"] as DateTime?,
                                Link = reader.GetString("AppLink"),
                                Name = reader.GetString("AppName"),
                                WikiLink = reader.GetString("WikiEntry"),
                                WikiVersion = reader.GetString("WikiVersion"),
                                LinkValid = reader.GetBoolean("AppLinkValid")
                            };
                            items.Add(item);
                        }
                        return items;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(nameof(GetAll), exception);
                return null;
            }
        }

        public static RequestResult Remove(int id)
        {
            var result = new RequestResult { Success = false };
            var items = GetAll();
            var item = items.FirstOrDefault(o => o.Id == id);
            if (item == null)
            {
                result.Message = "App does not exist";
                return result;
            }
            try
            {
                using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM appinfo WHERE Id=@Id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("Id", id);
                        var affected = command.ExecuteNonQuery();
                        if (affected == 1)
                        {
                            result.Success = true;
                            return result;
                        }

                        result.Message = "Remove failed";
                        return result;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Remove", exception);
                result.Message = "Exception - See log for details.";
                return result;
            }
        }
    }
}