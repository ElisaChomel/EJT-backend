using judo_backend.Models;
using judo_backend.Models.Enum;
using judo_backend.Services.Helper;
using judo_backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace judo_backend.Services
{
    public class UserService : BaseService, IUserService
    {
        private string secretKey;

        public UserService(IConfiguration configuration) : base(configuration) {
            this.secretKey = configuration.GetSection("SecretKey").Value;
        }

        public User Authenticate(Authenticate authenticate){
            var commandText = "SELECT * FROM Users WHERE (Email=@EMAIL OR Username=@USERANME) AND Password=@PASSWORD";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ Name = "@EMAIL", Value = authenticate.Email, Type = System.Data.DbType.String },
                new SqlParameter(){ Name = "@USERANME", Value = authenticate.Username, Type = System.Data.DbType.String },
                new SqlParameter(){ Name = "@PASSWORD", Value = GenerateMD5Password(authenticate.Password), Type = System.Data.DbType.String },
            };

            User user = this.GetUser(commandText, parameters);

            return user;
        }

        public User Get(int id)
        {
            var commandText = $"SELECT * FROM Users WHERE Id=@ID";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ Name = "@ID", Value = id, Type = System.Data.DbType.Int64 },
            };

            User user = this.GetUser(commandText, parameters);

            return user;
        }

        public User Get(string username)
        {
            var commandText = $"SELECT * FROM Users WHERE (Email=@USERANME OR Username=@USERANME)";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ Name = "@USERANME", Value = username, Type = System.Data.DbType.String },
            };

            User user = this.GetUser(commandText, parameters);

            return user;
        }

        public List<User> GetAll()
        {
            var commandeText = "SELECT ID, username, email, profile FROM Users";

            var list = this.GetUsers(commandeText);

            return list.OrderBy(x => x.Username).ToList();
        }

        public List<User> GetByAdherentId(int id)
        {
            var listId = new List<int>();
            var list = new List<User>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT User_ID FROM EjtAdherentLinkUser WHERE EjtAdherent_ID = @ADHERENTID";

                    cmd.Parameters.Add("@ADHERENTID", System.Data.DbType.Int16);
                    cmd.Parameters["@ADHERENTID"].Value = id;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listId.Add((int)reader["user_ID"]);
                        }
                    }
                }
            }

            foreach (var item in listId) 
            {
                list.Add(this.Get(item));
            }

            return list;
        }

        public User Create (Authenticate model)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO Users (username, email, password, profile) VALUES (@USERNAME, @EMAIL, @PASSWORD, 2); SELECT LAST_INSERT_ID();";
                    
                    cmd.Parameters.Add("@USERNAME", System.Data.DbType.String);
                    cmd.Parameters["@USERNAME"].Value = FormatValue(model.Username);

                    cmd.Parameters.Add("@EMAIL", System.Data.DbType.String);
                    cmd.Parameters["@EMAIL"].Value = FormatValue(model.Email);

                    cmd.Parameters.Add("@PASSWORD", System.Data.DbType.String);
                    cmd.Parameters["@PASSWORD"].Value = FormatValue(GenerateMD5Password(model.Password));

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    return this.Get(id);
                }
            }
        }

        public void CreateLink(int userId, int adherentId)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO EjtAdherentLinkUser (User_ID, EjtAdherent_ID) VALUES (@USERID, @ADHERENTID);";

                    cmd.Parameters.Add("@USERID", System.Data.DbType.Int16);
                    cmd.Parameters["@USERID"].Value = userId;

                    cmd.Parameters.Add("@ADHERENTID", System.Data.DbType.Int16);
                    cmd.Parameters["@ADHERENTID"].Value = adherentId;

                    cmd.ExecuteScalar();
                }
            }
        }

        public User Update(User u)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Users SET username = @USERNAME, email = @EMAIL, profile = @PROFILE WHERE id = {u.Id}";
                    
                    cmd.Parameters.Add("@USERNAME", System.Data.DbType.String);
                    cmd.Parameters["@USERNAME"].Value = FormatValue(u.Username);

                    cmd.Parameters.Add("@EMAIL", System.Data.DbType.String);
                    cmd.Parameters["@EMAIL"].Value = FormatValue(u.Email);

                    cmd.Parameters.Add("@PROFILE", System.Data.DbType.UInt16);
                    cmd.Parameters["@PROFILE"].Value = (int)u.Profile;

                    cmd.ExecuteScalar();

                    return this.Get(u.Id);
                }
            }
        }

        public User UpdatePassword(int id, string password)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Users SET password = @PASSWORD WHERE id = {id}";

                    cmd.Parameters.Add("@PASSWORD", System.Data.DbType.String);
                    cmd.Parameters["@PASSWORD"].Value = FormatValue(GenerateMD5Password(password));

                    cmd.ExecuteScalar();

                    return this.Get(id);
                }
            }
        }

        public void Delete(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"DELETE FROM Users WHERE id = {id}";

                    cmd.ExecuteScalar();
                }
            }
        }
        public void DeleteLink(int userId)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"DELETE FROM EjtAdherentLinkUser WHERE User_ID = {userId}";

                    cmd.ExecuteScalar();
                }
            }
        }

        public bool CheckPassword(int id, string password)
        {
            var commandText = $"SELECT * FROM Users WHERE ID=@ID AND Password=@PASSWORD";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ Name = "@ID", Value = id, Type = System.Data.DbType.Int64 },
                new SqlParameter(){ Name = "@PASSWORD", Value = GenerateMD5Password(password), Type = System.Data.DbType.String },
            };

            User user = this.GetUser(commandText, parameters);

            return user != null;
        }

        private User GetUser(string commandText, List<SqlParameter> parameters)
        {
            User user = null;

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = commandText;

                    foreach (SqlParameter param in parameters)
                    {
                        cmd.Parameters.Add(param.Name, param.Type);
                        cmd.Parameters[param.Name].Value = param.Value;
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var id = (int)reader["id"];
                            user = new User()
                            {
                                Id = id,
                                Username = (string)reader["username"],
                                Email = (string)reader["email"],
                                Profile = (Profile)(int)reader["profile"],
                                Token = GenerateJwtToken(id)
                            };
                        }
                    }
                }
            }

            return user;
        }

        private List<User> GetUsers(string commandText)
        {
            var list = new List<User>();

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
                            list.Add(new User()
                            {
                                Id = (int)reader["id"],
                                Username = (string)reader["username"],
                                Email = (string)reader["email"],
                                Profile = (Profile)reader["profile"]
                            });
                        }
                    }
                }
            }

            return list;
        }

        private string GenerateMD5Password(string s)
        {
            StringBuilder sb = new StringBuilder();

            // Initialize a MD5 hash object
            using (MD5 md5 = MD5.Create())
            {
                // Compute the hash of the given string
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(s));

                // Convert the byte array to string format
                foreach (byte b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }

            return sb.ToString();
        }

        private string GenerateJwtToken(int id)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
