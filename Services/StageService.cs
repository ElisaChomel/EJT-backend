using judo_backend.Models;
using judo_backend.Models.Enum;
using judo_backend.Services.Interfaces;
using MySqlConnector;

namespace judo_backend.Services
{
    public class StageService : BaseService, IStageService
    {
        public StageService(IConfiguration configuration) : base(configuration)
        {
           
        }

        public List<Stage> GetAll()
        {
            var commandText = "SELECT ID, startdate, enddate, name, address, yearBirthdayMin, yearBirthdayMax, maxInscriptionDate FROM Stages";

            var list = this.GetList(commandText);

            return list.OrderByDescending(x => x.Start).ThenBy(x => x.Name).ToList();
        }

        public List<Stage> GetAllActive()
        {
            var commandText = $"SELECT ID, startdate, enddate, name, address, yearBirthdayMin, yearBirthdayMax, maxInscriptionDate FROM Stages WHERE maxInscriptionDate > '{DateTime.Now.ToString("yyyy-MM-dd")}'";

            var list = this.GetList(commandText);

            return list.OrderByDescending(x => x.Start).ThenBy(x => x.Name).ToList();
        }

        public List<int> GetStagesInscription(int adherentId)
        {
            var list = new List<int>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT Stages_ID FROM StagesInscriptions WHERE Adherent_ID = {adherentId}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add((int)reader["Stages_ID"]);
                        }
                    }
                }
            }

            return list;
        }

        public List<EjtAdherent> GetAdherentsInscription(int id)
        {
            var list = new List<EjtAdherent>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ejtadherent.* FROM ejtadherent JOIN stagesinscriptions ON stagesinscriptions.Adherent_ID = ejtadherent.ID WHERE stagesinscriptions.Stages_ID = {id}";

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

        public Stage Get(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ID, startdate, enddate, name, address, yearBirthdayMin, yearBirthdayMax, maxInscriptionDate FROM Stages WHERE ID={id}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Stage()
                            {
                                Id = (int)reader["id"],
                                Start = (DateTime)reader["startDate"],
                                End = (DateTime)reader["endDate"],
                                Name = (string)reader["name"],
                                Address = (string)reader["address"],
                                YearBirthdayMin = (int)reader["yearBirthdayMin"],
                                YearBirthdayMax = (int)reader["yearBirthdayMax"],
                                MaxInscriptionDate = (DateTime)reader["maxInscriptionDate"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public Stage Create(Stage s)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO Stages (startDate, endDate, name, address, yearBirthdayMin, yearBirthdayMax, maxInscriptionDate) VALUES (@START, @END, @NAME, @ADDRESS, @YBMIN, @YBMAX, @MAXDATE); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@START", System.Data.DbType.Date);
                    cmd.Parameters["@START"].Value = s.Start;

                    cmd.Parameters.Add("@END", System.Data.DbType.Date);
                    cmd.Parameters["@END"].Value = s.End;

                    cmd.Parameters.Add("@NAME", System.Data.DbType.String);
                    cmd.Parameters["@NAME"].Value = FormatValue(s.Name);

                    cmd.Parameters.Add("@ADDRESS", System.Data.DbType.String);
                    cmd.Parameters["@ADDRESS"].Value = FormatValue(s.Address);

                    cmd.Parameters.Add("@YBMIN", System.Data.DbType.Int16);
                    cmd.Parameters["@YBMIN"].Value = s.YearBirthdayMin;

                    cmd.Parameters.Add("@YBMAX", System.Data.DbType.Int16);
                    cmd.Parameters["@YBMAX"].Value = s.YearBirthdayMax;

                    cmd.Parameters.Add("@MAXDATE", System.Data.DbType.Date);
                    cmd.Parameters["@MAXDATE"].Value = s.MaxInscriptionDate;

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public void CreateStageInscription(int adherentId, int stageId)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO StagesInscriptions (Stages_ID, Adherent_ID) VALUES (@STAGEID, @ADHERENTID); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@STAGEID", System.Data.DbType.Int16);
                    cmd.Parameters["@STAGEID"].Value = stageId;

                    cmd.Parameters.Add("@ADHERENTID", System.Data.DbType.Int64);
                    cmd.Parameters["@ADHERENTID"].Value = adherentId;

                    cmd.ExecuteScalar();
                }
            }
        }

        public Stage Update(Stage s)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Stages SET startDate = @START, endDate = @END, name = @NAME, address = @ADDRESS, yearBirthdayMin = @YBMIN, yearBirthdayMax = @YBMAX, maxInscriptionDate = @MAXDATE WHERE id = {s.Id};";

                    cmd.Parameters.Add("@START", System.Data.DbType.Date);
                    cmd.Parameters["@START"].Value = s.Start;

                    cmd.Parameters.Add("@END", System.Data.DbType.Date);
                    cmd.Parameters["@END"].Value = s.End;

                    cmd.Parameters.Add("@NAME", System.Data.DbType.String);
                    cmd.Parameters["@NAME"].Value = FormatValue(s.Name);

                    cmd.Parameters.Add("@ADDRESS", System.Data.DbType.String);
                    cmd.Parameters["@ADDRESS"].Value = FormatValue(s.Address);

                    cmd.Parameters.Add("@YBMIN", System.Data.DbType.Int16);
                    cmd.Parameters["@YBMIN"].Value = s.YearBirthdayMin;

                    cmd.Parameters.Add("@YBMAX", System.Data.DbType.Int16);
                    cmd.Parameters["@YBMAX"].Value = s.YearBirthdayMax;

                    cmd.Parameters.Add("@MAXDATE", System.Data.DbType.Date);
                    cmd.Parameters["@MAXDATE"].Value = s.MaxInscriptionDate;

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        private List<Stage> GetList(string commandText)
        {
            var list = new List<Stage>();

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
                            list.Add(new Stage()
                            {
                                Id = (int)reader["id"],
                                Start = (DateTime)reader["startDate"],
                                End = (DateTime)reader["endDate"],
                                Name = (string)reader["name"],
                                Address = (string)reader["address"],
                                YearBirthdayMin = (int)reader["yearBirthdayMin"],
                                YearBirthdayMax = (int)reader["yearBirthdayMax"],
                                MaxInscriptionDate = (DateTime)reader["maxInscriptionDate"]
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
