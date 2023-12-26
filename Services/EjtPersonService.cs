using judo_backend.Models.Enum;
using judo_backend.Models;
using judo_backend.Services.Interfaces;
using MySqlConnector;

namespace judo_backend.Services
{
    public class EjtPersonService : BaseService, IEjtPersonService
    {
        public EjtPersonService(IConfiguration configuration) : base(configuration)
        {
        }

        public EjtPerson Get(int id)
        {
            var commandText = $"SELECT * FROM EjtPerson WHERE Id={id}";

            EjtPerson person = null;

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = commandText;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            person = new EjtPerson()
                            {
                                Id = (int)reader["id"],
                                Name = (string)reader["name"],
                                Type = (EjtPersonType)reader["type"],
                                Role = (string)reader["role"],
                                Detail = (string)reader["detail"],
                                PhotoName = (string)reader["photoname"]
                            };
                        }
                    }
                }
            }

            return person;
        }

        public List<EjtPerson> GetAll()
        {
            var list = new List<EjtPerson>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM EjtPerson";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new EjtPerson()
                            {
                                Id = (int)reader["id"],
                                Name = (string)reader["name"],
                                Type = (EjtPersonType)reader["type"],
                                Role = (string)reader["role"],
                                Detail = (string)reader["detail"],
                                PhotoName = (string)reader["photoname"]
                            });
                        }
                    }
                }
            }

            return list.OrderBy(x => x.Name).ToList();
        }

        public EjtPerson Create(EjtPerson model)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO EjtPerson (name, type, role, detail, photoname) VALUES (@NAME, @TYPE, @ROLE, @DETAIL, @PHOTONAME); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@NAME", System.Data.DbType.String);
                    cmd.Parameters["@NAME"].Value = FormatValue(model.Name);

                    cmd.Parameters.Add("@TYPE", System.Data.DbType.Int64);
                    cmd.Parameters["@TYPE"].Value = (int)model.Type;

                    cmd.Parameters.Add("@ROLE", System.Data.DbType.String);
                    cmd.Parameters["@ROLE"].Value = FormatValue(model.Role);

                    cmd.Parameters.Add("@DETAIL", System.Data.DbType.String);
                    cmd.Parameters["@DETAIL"].Value = FormatValue(model.Detail);

                    cmd.Parameters.Add("@PHOTONAME", System.Data.DbType.String);
                    cmd.Parameters["@PHOTONAME"].Value = FormatValue(model.PhotoName);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public EjtPerson Update(EjtPerson model)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE EjtPerson SET name = @NAME, type = @TYPE, role = @ROLE, detail = @DETAIL, photoname = @PHOTONAME WHERE id = {model.Id}";

                    cmd.Parameters.Add("@NAME", System.Data.DbType.String);
                    cmd.Parameters["@NAME"].Value = FormatValue(model.Name);

                    cmd.Parameters.Add("@TYPE", System.Data.DbType.Int64);
                    cmd.Parameters["@TYPE"].Value = (int)model.Type;

                    cmd.Parameters.Add("@ROLE", System.Data.DbType.String);
                    cmd.Parameters["@ROLE"].Value = FormatValue(model.Role);

                    cmd.Parameters.Add("@DETAIL", System.Data.DbType.String);
                    cmd.Parameters["@DETAIL"].Value = FormatValue(model.Detail);

                    cmd.Parameters.Add("@PHOTONAME", System.Data.DbType.String);
                    cmd.Parameters["@PHOTONAME"].Value = FormatValue(model.PhotoName);

                    cmd.ExecuteScalar();

                    return this.Get(model.Id);
                }
            }
        }
    }
}
