using judo_backend.Models;
using judo_backend.Services.Interfaces;
using MySqlConnector;
using System.Collections.Generic;
using System.IO;

namespace judo_backend.Services
{
    public class Blob : BaseService, IBlob
    {
        public Blob(IConfiguration configuration) : base(configuration)
        {
        }

        public List<string> Get(string path)
        {
            var list = new List<string>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = "SELECT filename FROM Files WHERE path=@PATH";

                    cmd.Parameters.Add("@PATH", System.Data.DbType.String);
                    cmd.Parameters["@PATH"].Value = path;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add((string)reader["filename"]);
                        }
                    }
                }
            }

            return list.OrderBy(x => int.Parse(x.Replace("photo-", "").Replace(".jpg", ""))).ToList();
        }

        public MemoryStream Get(string path, string filename)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT file FROM Files WHERE path=@PATH AND filename=@FILENAME";

                    cmd.Parameters.Add("@PATH", System.Data.DbType.String);
                    cmd.Parameters["@PATH"].Value = path;

                    cmd.Parameters.Add("@FILENAME", System.Data.DbType.String);
                    cmd.Parameters["@FILENAME"].Value = filename;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            byte[] bytes = (byte[])reader["file"];
                            MemoryStream stream = new MemoryStream();
                            stream.Write(bytes, 0, bytes.Length);

                            return stream;
                        }
                    }
                }
            }

            return null;
        }

        public void Change(string path, string fileName1, string fileName2)
        {
            IDictionary<int, string> dico = new Dictionary<int, string>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ID, fileName FROM Files WHERE path=@PATH AND (filename=@FILENAME1 OR filename=@FILENAME2)";

                    cmd.Parameters.Add("@PATH", System.Data.DbType.String);
                    cmd.Parameters["@PATH"].Value = path;

                    cmd.Parameters.Add("@FILENAME1", System.Data.DbType.String);
                    cmd.Parameters["@FILENAME1"].Value = fileName1;

                    cmd.Parameters.Add("@FILENAME2", System.Data.DbType.String);
                    cmd.Parameters["@FILENAME2"].Value = fileName2;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dico.Add((int)reader["ID"], (string)reader["filename"]);
                        }
                    }
                }
            }

            foreach (var key in dico.Keys)
            {
               this.ChangeName(key, dico[key] == fileName1 ? fileName2 : fileName1);               
            }
        }

        public void Upload(string path, string filename, MemoryStream ms)
        {
            var files = this.Get(path, filename);

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    if(files == null)
                    {
                        cmd.CommandText = "INSERT INTO Files (path, filename, file) VALUES (@PATH, @FILENAME, @FILE);";
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE Files SET file=@FILE WHERE filename=@FILENAME;";
                    }

                    cmd.Parameters.Add("@PATH", System.Data.DbType.String);
                    cmd.Parameters["@PATH"].Value = path;

                    cmd.Parameters.Add("@FILENAME", System.Data.DbType.String);
                    cmd.Parameters["@FILENAME"].Value = filename;

                    cmd.Parameters.Add("@FILE", System.Data.DbType.Binary);
                    cmd.Parameters["@FILE"].Value = ms.ToArray();

                    cmd.ExecuteScalar();
                }
            }
        }

        public void Delete(string path, string filename)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Files WHERE path=@PATH AND filename=@FILENAME ";

                    cmd.Parameters.Add("@PATH", System.Data.DbType.String);
                    cmd.Parameters["@PATH"].Value = path;

                    cmd.Parameters.Add("@FILENAME", System.Data.DbType.String);
                    cmd.Parameters["@FILENAME"].Value = filename;

                    cmd.ExecuteScalar();
                }
            }
        }

        private void ChangeName(int id, string filename)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Files SET filename = @FILENAME WHERE ID = {id};";

                    cmd.Parameters.Add("@FILENAME", System.Data.DbType.String);
                    cmd.Parameters["@FILENAME"].Value = filename;

                    cmd.ExecuteScalar();
                }
            }
        }
    }
}
