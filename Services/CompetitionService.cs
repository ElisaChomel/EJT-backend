using judo_backend.Models;
using judo_backend.Models.Enum;
using judo_backend.Services.Interfaces;
using MySqlConnector;
using System.Security.Cryptography;

namespace judo_backend.Services
{
    public class CompetitionService : BaseService, ICompetitionService
    {
        public CompetitionService(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Competition> GetAll()
        {
            var commandText = "SELECT  ID, year, month, day, name, address, yearBirthdayMin, yearBirthdayMax, maxInscriptionDate  FROM Competitions";

            var list = this.GetList(commandText);

            return list.OrderByDescending(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day).ThenBy(x => x.Name).ToList();
        }

        public List<Competition> GetAllActive()
        {
            var commandText = $"SELECT DISTINCT competitions.ID, competitions.year, competitions.month , competitions.day, competitions.name, competitions.address, competitions.yearBirthdayMin, competitions.yearBirthdayMax, competitions.maxInscriptionDate FROM competitions LEFT JOIN competitionsresult ON competitions.ID = competitionsresult.Competition_ID WHERE competitionsresult.position IS NULL AND maxInscriptionDate > '{DateTime.Now.ToString("yyyy-MM-dd")}'";

            var list = this.GetList(commandText);

            return list.OrderByDescending(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day).ThenBy(x => x.Name).ToList();
        }

        public List<int> GetCompetitionsInscription(int adherentId)
        {
            var list = new List<int>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT DISTINCT competitionsresult.competition_ID FROM competitions LEFT JOIN competitionsresult ON competitions.ID = competitionsresult.Competition_ID WHERE competitionsresult.position IS NULL AND competitionsresult.Adherent_ID = {adherentId}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add((int)reader["competition_ID"]);
                        }
                    }
                }
            }

            return list;
        }

        public List<EjtAdherent> GetInscription(int id)
        {
            var list = new List<EjtAdherent>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ejtadherent.Id, ejtadherent.lastname, ejtadherent.firstname, ejtadherent.birthday, ejtadherent.licenceCode, ejtadherent.Weight, ejtadherent.Belt FROM competitionsresult JOIN ejtadherent ON ejtadherent.ID = competitionsresult.adherent_ID WHERE competitionsresult.competition_ID = {id}";

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

        public Competition Get(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ID, year, month, day, name, address, yearBirthdayMin, yearBirthdayMax, maxInscriptionDate FROM Competitions WHERE ID={id}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Competition()
                            {
                                Id = (int)reader["id"],
                                Year = (int)reader["year"],
                                Month = (int)reader["month"],
                                Day = (int)reader["day"],
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
        
        public CompetitionResult GetResult(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT competitionsresult.ID, competitionsresult.competition_ID, competitionsresult.adherent_ID, ejtadherent.firstname, ejtadherent.lastname, competitionsresult.POSITION FROM competitionsresult JOIN ejtadherent ON ejtadherent.ID = competitionsresult.adherent_ID WHERE competitionsresult.ID = {id}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CompetitionResult()
                            {
                                Id = (int)reader["id"],
                                Competition_ID = (int)reader["competition_ID"],
                                Adherent_ID = (int)reader["adherent_ID"],
                                Firstname = (string)reader["firstname"],
                                Name = (string)reader["lastname"],
                                Position = !reader.IsDBNull(5)? (int?)reader["position"]: null
                            };
                        }
                    }
                }
            }

            return null;
        }
       
        public List<CompetitionResult> GetResults(int id)
        {
            var list = new List<CompetitionResult>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT competitionsresult.ID, competitionsresult.competition_ID, competitionsresult.adherent_ID, ejtadherent.firstname, ejtadherent.lastname, competitionsresult.POSITION FROM competitionsresult JOIN ejtadherent ON ejtadherent.ID = competitionsresult.adherent_ID WHERE competitionsresult.competition_ID = {id}";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CompetitionResult()
                            {
                                Id = (int)reader["id"],
                                Competition_ID = (int)reader["competition_ID"],
                                Adherent_ID = (int)reader["adherent_ID"],
                                Firstname = (string)reader["firstname"],
                                Name = (string)reader["lastname"],
                                Position = !reader.IsDBNull(5) ? (int?)reader["position"] : null
                            });
                        }
                    }
                }
            }

            return list.OrderBy(x => x.Position).ToList();
        }

        public Competition Create(Competition c)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO Competitions (year, month, day, name, address, yearBirthdayMin, yearBirthdayMax, maxInscriptionDate) VALUES (@YEAR, @MONTH, @DAY, @NAME, @ADDRESS, @YBMIN, @YBMAX, @MAXDATE); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@YEAR", System.Data.DbType.Int16);
                    cmd.Parameters["@YEAR"].Value = c.Year;

                    cmd.Parameters.Add("@MONTH", System.Data.DbType.Int16);
                    cmd.Parameters["@MONTH"].Value = c.Month;

                    cmd.Parameters.Add("@DAY", System.Data.DbType.Int16);
                    cmd.Parameters["@DAY"].Value = c.Day;

                    cmd.Parameters.Add("@NAME", System.Data.DbType.String);
                    cmd.Parameters["@NAME"].Value = FormatValue(c.Name);

                    cmd.Parameters.Add("@ADDRESS", System.Data.DbType.String);
                    cmd.Parameters["@ADDRESS"].Value = FormatValue(c.Address);

                    cmd.Parameters.Add("@YBMIN", System.Data.DbType.Int16);
                    cmd.Parameters["@YBMIN"].Value = c.YearBirthdayMin;

                    cmd.Parameters.Add("@YBMAX", System.Data.DbType.Int16);
                    cmd.Parameters["@YBMAX"].Value = c.YearBirthdayMax;

                    cmd.Parameters.Add("@MAXDATE", System.Data.DbType.Date);
                    cmd.Parameters["@MAXDATE"].Value = c.MaxInscriptionDate;

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public CompetitionResult CreateResult(CompetitionResult c)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    if (c.Position.HasValue)
                    {
                        cmd.CommandText = $"INSERT INTO CompetitionsResult (competition_id, position, adherent_id) VALUES (@COMPETITIONID, @POSITION, @ADHERENTID); SELECT LAST_INSERT_ID();";

                        cmd.Parameters.Add("@POSITION", System.Data.DbType.Int64);
                        cmd.Parameters["@POSITION"].Value = c.Position;
                    }
                    else
                    {
                        cmd.CommandText = $"INSERT INTO CompetitionsResult (competition_id, adherent_id) VALUES (@COMPETITIONID, @ADHERENTID); SELECT LAST_INSERT_ID();";
                    }

                    cmd.Parameters.Add("@COMPETITIONID", System.Data.DbType.Int16);
                    cmd.Parameters["@COMPETITIONID"].Value = c.Competition_ID;

                    cmd.Parameters.Add("@ADHERENTID", System.Data.DbType.Int64);
                    cmd.Parameters["@ADHERENTID"].Value = c.Adherent_ID;

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.GetResult(id);
                }
            }
        }

        public Competition Update(Competition c)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Competitions SET year = @YEAR, month = @MONTH, day = @DAY, name = @NAME, address = @ADDRESS, yearBirthdayMin = @YBMIN, yearBirthdayMax = @YBMAX, maxInscriptionDate = @MAXDATE WHERE id = {c.Id};";

                    cmd.Parameters.Add("@YEAR", System.Data.DbType.Int16);
                    cmd.Parameters["@YEAR"].Value = c.Year;

                    cmd.Parameters.Add("@MONTH", System.Data.DbType.Int16);
                    cmd.Parameters["@MONTH"].Value = c.Month;

                    cmd.Parameters.Add("@DAY", System.Data.DbType.Int16);
                    cmd.Parameters["@DAY"].Value = c.Day;

                    cmd.Parameters.Add("@NAME", System.Data.DbType.String);
                    cmd.Parameters["@NAME"].Value = FormatValue(c.Name);

                    cmd.Parameters.Add("@ADDRESS", System.Data.DbType.String);
                    cmd.Parameters["@ADDRESS"].Value = FormatValue(c.Address);

                    cmd.Parameters.Add("@YBMIN", System.Data.DbType.Int16);
                    cmd.Parameters["@YBMIN"].Value = c.YearBirthdayMin;

                    cmd.Parameters.Add("@YBMAX", System.Data.DbType.Int16);
                    cmd.Parameters["@YBMAX"].Value = c.YearBirthdayMax;

                    cmd.Parameters.Add("@MAXDATE", System.Data.DbType.Date);
                    cmd.Parameters["@MAXDATE"].Value = c.MaxInscriptionDate;

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public CompetitionResult UpdateResult(CompetitionResult c)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE CompetitionsResult SET position = @POSITION, adherent_id = @ADHERENTID WHERE id = {c.Id};";

                    cmd.Parameters.Add("@POSITION", System.Data.DbType.Int64);
                    cmd.Parameters["@POSITION"].Value = c.Position;

                    cmd.Parameters.Add("@ADHERENTID", System.Data.DbType.Int64);
                    cmd.Parameters["@ADHERENTID"].Value = c.Adherent_ID;

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.GetResult(id);
                }
            }
        }

        private List<Competition> GetList(string commandText)
        {
            var list = new List<Competition>();

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
                            list.Add(new Competition()
                            {
                                Id = (int)reader["id"],
                                Year = (int)reader["year"],
                                Month = (int)reader["month"],
                                Day = (int)reader["day"],
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
