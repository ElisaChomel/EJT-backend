using judo_backend.Models;
using judo_backend.Services.Interfaces;
using MySqlConnector;

namespace judo_backend.Services
{
    public class AgendaService : BaseService, IAgendaService
    {
        public AgendaService(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Agenda> GetAll()
        {
            var list = new List<Agenda>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID, Date, Title, Resume, Detail, Address FROM Agenda";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Agenda()
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                Title = (string)reader["title"],
                                Resume = (string)reader["resume"],
                                Detail = (string)reader["detail"],
                                Address = (string)reader["address"]
                            });
                        }
                    }
                }
            }

            return list.OrderByDescending(x => x.Date).ToList();
        }

        public Agenda Get(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ID, Date, Title, Resume, Detail, Address FROM Agenda WHERE ID={id}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Agenda()
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                Title = (string)reader["title"],
                                Resume = (string)reader["resume"],
                                Detail = (string)reader["detail"],
                                Address = (string)reader["address"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public Agenda Upload(Agenda a)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO Agenda (date, title, resume, detail, address) VALUES (@DATE, @TITLE, @RESUME, @DETAIL, @ADDRESS); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@DATE", System.Data.DbType.DateTime);
                    cmd.Parameters["@DATE"].Value = a.Date;

                    cmd.Parameters.Add("@TITLE", System.Data.DbType.String);
                    cmd.Parameters["@TITLE"].Value = FormatValue(a.Title);

                    cmd.Parameters.Add("@RESUME", System.Data.DbType.String);
                    cmd.Parameters["@RESUME"].Value = FormatValue(a.Resume);

                    cmd.Parameters.Add("@DETAIL", System.Data.DbType.String);
                    cmd.Parameters["@DETAIL"].Value = FormatValue(a.Detail);

                    cmd.Parameters.Add("@ADDRESS", System.Data.DbType.String);
                    cmd.Parameters["@ADDRESS"].Value = FormatValue(a.Address);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public void Update(Agenda a)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Agenda SET date = @DATE, title = @TITLE, resume = @RESUME, detail = @DETAIL, address=@ADDRESS WHERE id = {a.Id}";
                    
                    cmd.Parameters.Add("@DATE", System.Data.DbType.DateTime);
                    cmd.Parameters["@DATE"].Value = a.Date;

                    cmd.Parameters.Add("@TITLE", System.Data.DbType.String);
                    cmd.Parameters["@TITLE"].Value = FormatValue(a.Title);

                    cmd.Parameters.Add("@RESUME", System.Data.DbType.String);
                    cmd.Parameters["@RESUME"].Value = FormatValue(a.Resume);

                    cmd.Parameters.Add("@DETAIL", System.Data.DbType.String);
                    cmd.Parameters["@DETAIL"].Value = FormatValue(a.Detail);

                    cmd.Parameters.Add("@ADDRESS", System.Data.DbType.String);
                    cmd.Parameters["@ADDRESS"].Value = FormatValue(a.Address);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
