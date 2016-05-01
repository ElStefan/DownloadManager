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
                Log.Error("Insert - Invalid app object");
                result.Message = "Invalid app object";
                return result;
            }
            var items = GetAll();
            var item = items.FirstOrDefault(o => o.Name == app.Name && o.Link == app.Link);
            if (item != null)
            {
                Log.Error("Insert - Database item already exists");
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
                    var query = "INSERT INTO appInfos (app_name,app_link,lastupdate,wikiEntry,applinkValid) VALUES (@app_name,@app_link,null,@wikiEntry,@applinkValid);";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        var linkValid = 0;
                        if (app.LinkValid)
                        {
                            linkValid = 1;
                        }
                        command.Parameters.AddWithValue("app_name", app.Name);
                        command.Parameters.AddWithValue("app_link", app.Link);
                        command.Parameters.AddWithValue("wikiEntry", app.WikiLink);
                        command.Parameters.AddWithValue("applinkValid", linkValid);
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
                Log.Error("Update - Invalid app object");
                result.Message = "Invalid app object";
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
                    var query = "UPDATE appInfos SET app_name=@app_name, app_link=@app_link, lastupdate=@lastupdate, wikiEntry=@wikiEntry, wikiVersion=@wikiVersion, applinkValid=@applinkValid WHERE id=@id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        var linkValid = 0;
                        if (app.LinkValid)
                        {
                            linkValid = 1;
                        }
                        command.Parameters.AddWithValue("id", app.Id);
                        command.Parameters.AddWithValue("app_link", app.Link);
                        command.Parameters.AddWithValue("applinkValid", linkValid);
                        command.Parameters.AddWithValue("app_name", app.Name);
                        command.Parameters.AddWithValue("lastupdate", DateTime.Now);
                        command.Parameters.AddWithValue("wikiEntry", app.WikiLink);
                        command.Parameters.AddWithValue("wikiVersion", app.WikiVersion);
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
                    var query = "SELECT id, app_name, app_link, applinkValid, lastupdate, wikiEntry, wikiVersion FROM appInfos";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        var reader = command.ExecuteReader();
                        var items = new List<AppInfo>();
                        while (reader.Read())
                        {
                            var tempValid = reader["applinkValid"] as int?;
                            var item = new AppInfo
                            {
                                Id = (reader["id"] as int?).Value,
                                LastUpdate = reader["lastupdate"] as DateTime?,
                                Link = reader["app_link"] as string,
                                Name = reader["app_name"] as string,
                                WikiLink = reader["wikiEntry"] as string,
                                WikiVersion = reader["wikiVersion"] as string
                            };
                            if (tempValid.HasValue)
                            {
                                item.LinkValid = tempValid.Value > 0;
                            }
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
                    var query = "DELETE FROM appInfos WHERE id=@id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
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