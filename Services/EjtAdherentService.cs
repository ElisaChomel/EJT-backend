using judo_backend.Models;
using judo_backend.Models.Enum;
using judo_backend.Services.Interfaces;
using MySqlConnector;

namespace judo_backend.Services
{
    public class EjtAdherentService : BaseService, IEjtAdherentService
    {
        public EjtAdherentService(IConfiguration configuration) : base(configuration)
        {
        }

        public EjtAdherent Get(int id)
        {
            var commandText = $"SELECT * FROM EjtAdherent WHERE Id={id}";

            return this.GetEjtAdherent(commandText);
        }

        public EjtAdherent Get(string licenceCode)
        {
            var commandText = $"SELECT * FROM EjtAdherent WHERE licenceCode = '{licenceCode}'";

            return this.GetEjtAdherent(commandText);
        }

        public List<EjtAdherent> GetAll()
        {
            var commandText = "SELECT * FROM EjtAdherent";

            var list = this.GetAllEjtAdherent(commandText);

            return list.OrderBy(x => x.Lastname).ThenBy(x => x.Firstname).ToList();
        }

        public List<EjtAdherent> GetAllByUserId(int userId)
        {
            var commandText = $"SELECT ejtadherent.* FROM ejtadherent JOIN ejtadherentlinkuser ON ejtadherent.ID = ejtadherentlinkuser.EjtAdherent_ID JOIN users ON users.ID = ejtadherentlinkuser.User_ID WHERE users.ID = {userId}";

            var list = this.GetAllEjtAdherent(commandText);

            return list.OrderBy(x => x.Lastname).ThenBy(x => x.Firstname).ToList();

        }

        public EjtAdherent Create(EjtAdherent model)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO EjtAdherent (lastname, firstname, birthday, licenceCode, weight, belt) VALUES (@LASTNAME, @FIRSTNAME, @BIRTHDAY, @LICENCECODE, @WEIGHT, @BELT); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@LASTNAME", System.Data.DbType.String);
                    cmd.Parameters["@LASTNAME"].Value = FormatValue(model.Lastname);

                    cmd.Parameters.Add("@FIRSTNAME", System.Data.DbType.String);
                    cmd.Parameters["@FIRSTNAME"].Value = FormatValue(model.Firstname);

                    cmd.Parameters.Add("@BIRTHDAY", System.Data.DbType.Date);
                    cmd.Parameters["@BIRTHDAY"].Value = model.Birthday;

                    cmd.Parameters.Add("@LICENCECODE", System.Data.DbType.String);
                    cmd.Parameters["@LICENCECODE"].Value = FormatValue(model.LicenceCode);

                    cmd.Parameters.Add("@WEIGHT", System.Data.DbType.VarNumeric);
                    cmd.Parameters["@WEIGHT"].Value = model.Weight;

                    cmd.Parameters.Add("@BELT", System.Data.DbType.Int64);
                    cmd.Parameters["@BELT"].Value = model.Belt;

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public EjtAdherent Update(EjtAdherent model)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE EjtAdherent SET lastname = @LASTNAME, firstname = @FIRSTNAME, birthday = @BIRTHDAY, licenceCode = @LICENCECODE, weight = @WEIGHT, belt = @BELT WHERE id = {model.Id}";

                    cmd.Parameters.Add("@LASTNAME", System.Data.DbType.String);
                    cmd.Parameters["@LASTNAME"].Value = FormatValue(model.Lastname);

                    cmd.Parameters.Add("@FIRSTNAME", System.Data.DbType.String);
                    cmd.Parameters["@FIRSTNAME"].Value = FormatValue(model.Firstname);

                    cmd.Parameters.Add("@BIRTHDAY", System.Data.DbType.Date);
                    cmd.Parameters["@BIRTHDAY"].Value = model.Birthday;

                    cmd.Parameters.Add("@LICENCECODE", System.Data.DbType.String);
                    cmd.Parameters["@LICENCECODE"].Value = FormatValue(model.LicenceCode);

                    cmd.Parameters.Add("@WEIGHT", System.Data.DbType.VarNumeric);
                    cmd.Parameters["@WEIGHT"].Value = model.Weight;

                    cmd.Parameters.Add("@BELT", System.Data.DbType.Int64);
                    cmd.Parameters["@BELT"].Value = (int)model.Belt;

                    cmd.ExecuteScalar();

                    return this.Get(model.Id);
                }
            }
        }

        private EjtAdherent GetEjtAdherent(string commandText)
        {
            EjtAdherent adherent = null;

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
                            adherent = new EjtAdherent()
                            {
                                Id = (int)reader["id"],
                                Lastname = (string)reader["lastname"],
                                Firstname = (string)reader["firstname"],
                                Birthday = (DateTime)reader["birthday"],
                                LicenceCode = (string)reader["licenceCode"],
                                Weight = reader.IsDBNull(5) ? null : (float)reader["weight"],
                                Belt = reader.IsDBNull(6) ? null : (BeltType)reader["belt"]
                            };
                        }
                    }
                }
            }

            return adherent;
        }

        private List<EjtAdherent> GetAllEjtAdherent(string commandText)
        {
            var list = new List<EjtAdherent>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = commandText;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new EjtAdherent()
                            {
                                Id = (int)reader["id"],
                                Lastname = (string)reader["lastname"],
                                Firstname = (string)reader["firstname"],
                                Birthday = (DateTime)reader["birthday"],
                                LicenceCode = (string)reader["licenceCode"],
                                Weight = reader.IsDBNull(5) ? null : (float)reader["weight"],
                                Belt = reader.IsDBNull(6) ? null : (BeltType)reader["belt"]
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}