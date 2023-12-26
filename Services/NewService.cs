using judo_backend.Models;
using judo_backend.Models.Enum;
using judo_backend.Services.Interfaces;
using MySqlConnector;

namespace judo_backend.Services
{
    public class NewService : BaseService, INewService
    {
        public NewService(IConfiguration configuration) : base(configuration)
        {
        }

        public List<New> GetAll()
        {
            var list = new List<New>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID, Date, Title, Resume, Detail FROM News";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new New()
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                Title = (string)reader["title"],
                                Resume = (string)reader["resume"],
                                Detail = (string)reader["detail"]
                            });
                        }
                    }
                }
            }

            return list.OrderByDescending(x => x.Date).ToList();
        }

        public New Get(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ID, Date, Title, Resume, Detail FROM News WHERE ID={id}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new New()
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                Title = (string)reader["title"],
                                Resume = (string)reader["resume"],
                                Detail = (string)reader["detail"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public New Upload(New n)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO News (date, title, resume, detail) VALUES (@DATE, @TITLE, @RESUME, @DETAIL); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@DATE", System.Data.DbType.DateTime);
                    cmd.Parameters["@DATE"].Value = n.Date;

                    cmd.Parameters.Add("@TITLE", System.Data.DbType.String);
                    cmd.Parameters["@TITLE"].Value = FormatValue(n.Title);

                    cmd.Parameters.Add("@RESUME", System.Data.DbType.String);
                    cmd.Parameters["@RESUME"].Value = FormatValue(n.Resume);

                    cmd.Parameters.Add("@DETAIL", System.Data.DbType.String);
                    cmd.Parameters["@DETAIL"].Value = FormatValue(n.Detail);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public void Update(New n)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE News SET date = @DATE, title = @TITLE, resume = @RESUME, detail = @DETAIL WHERE id = {n.Id}";

                    cmd.Parameters.Add("@DATE", System.Data.DbType.DateTime);
                    cmd.Parameters["@DATE"].Value = n.Date;

                    cmd.Parameters.Add("@TITLE", System.Data.DbType.String);
                    cmd.Parameters["@TITLE"].Value = FormatValue(n.Title);

                    cmd.Parameters.Add("@RESUME", System.Data.DbType.String);
                    cmd.Parameters["@RESUME"].Value = FormatValue(n.Resume);

                    cmd.Parameters.Add("@DETAIL", System.Data.DbType.String);
                    cmd.Parameters["@DETAIL"].Value = FormatValue(n.Detail);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
