using judo_backend.Models.Stats;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace judo_backend.Services
{
    public class StatsService : BaseService, IStatsService
    {
        public StatsService(IConfiguration configuration) : base(configuration)
        {
        }

        public Stats Get(DateTime date, string pageName)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ID, date, namePage, countView FROM Stat WHERE date=@DATE AND namePage=@PAGENAME";

                    cmd.Parameters.Add("@DATE", System.Data.DbType.Date);
                    cmd.Parameters["@DATE"].Value = date;

                    cmd.Parameters.Add("@PAGENAME", System.Data.DbType.String);
                    cmd.Parameters["@PAGENAME"].Value = FormatValue(pageName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Stats()
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                PageName = (string)reader["namePage"],
                                CountView = (int)reader["countView"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public List<string> GetPageNames(DateTime date)
        {
            List<string> names = new List<string>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT namePage FROM Stat WHERE date=@DATE  ORDER BY namePage";

                    cmd.Parameters.Add("@DATE", System.Data.DbType.Date);
                    cmd.Parameters["@DATE"].Value = date;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            names.Add((string)reader["namePage"]);
                        }
                    }
                }
            }

            return names;
        }

        public Stats Get(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ID, date, namePage, countView FROM Stat WHERE id= {id}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Stats()
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                PageName = (string)reader["namePage"],
                                CountView = (int)reader["countView"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public PieValues GetPieValues()
        {
            PieValues pieValues = new PieValues();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT namePage, SUM(CountView) AS CountView FROM stat GROUP BY namePage ORDER BY namePage";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var value = int.Parse(reader["countView"].ToString());
                            pieValues.Labels.Add($"{(string)reader["namePage"]} ({value})");
                            pieValues.Values.Add(value);
                        }
                    }
                }
            }

            return pieValues;
        }

        public Stats Create(DateTime date, string pageName)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO Stat (date, namePage, countView) VALUES (@DATE, @PAGENAME, 1); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@DATE", System.Data.DbType.Date);
                    cmd.Parameters["@DATE"].Value = date;

                    cmd.Parameters.Add("@PAGENAME", System.Data.DbType.String);
                    cmd.Parameters["@PAGENAME"].Value = FormatValue(pageName);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public Stats Update(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Stat SET countView = countView + 1 WHERE id = {id};";

                    cmd.ExecuteScalar();

                    return this.Get(id);
                }
            }
        }

    }
}
