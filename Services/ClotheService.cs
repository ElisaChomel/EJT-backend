using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using judo_backend.Models;
using judo_backend.Services.Interfaces;
using MySqlConnector;
using System.Security.Cryptography.Xml;

namespace judo_backend.Services
{
    public class ClotheService : BaseService, IClotheService
    {
        public ClotheService(IConfiguration configuration) : base(configuration)
        {
        }

        public DateTime GetDate()
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT Date FROM clothesOrderDate WHERE Id = 1";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (DateTime)reader["date"];
                        }
                    }
                }
            }

            return DateTime.Now;
        }

        public DateTime SetDate(DateTime date)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE clothesorderdate SET Date=@DATE WHERE Id = 1";

                    cmd.Parameters.Add("@DATE", System.Data.DbType.DateTime);
                    cmd.Parameters["@DATE"].Value = date;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (DateTime)reader["date"];
                        }
                    }
                }
            }

            return date;
        }

        public Clothe Get(string reference)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT ID, Ref, TargetPoeple, JacketDescription, PantDescription, Description, Composition, Price, Size FROM Clothes WHERE Ref=@REF";

                    cmd.Parameters.Add("@REF", System.Data.DbType.String);
                    cmd.Parameters["@REF"].Value = reference;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Clothe()
                            {
                                Id = (int)reader["id"],
                                Reference = (string)reader["ref"],
                                TargetPoeple = (string)reader["targetPoeple"],
                                JacketDescription = reader.IsDBNull(3) ? null : (string)reader["jacketDescription"],
                                PantDescription = reader.IsDBNull(4) ? null : (string)reader["pantDescription"],
                                Description = reader.IsDBNull(5) ? null : (string)reader["description"],
                                Composition = (string)reader["composition"],
                                Price = (int)reader["price"],
                                Size = (string)reader["size"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        public MemoryStream GetFile(string reference)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT file FROM Clothes WHERE Ref=@REF";

                    cmd.Parameters.Add("@REF", System.Data.DbType.String);
                    cmd.Parameters["@REF"].Value = reference;

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

        public List<ClotheOrder> GetAll()
        {
            var list = new List<ClotheOrder>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID, email, ref, price, isPay FROM ClothesOrder WHERE isDelete = 0";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ClotheOrder()
                            {
                                Id = (int)reader["id"],
                                Email = (string)reader["email"],
                                Reference = (string)reader["ref"],
                                Price = (int)reader["price"],
                                IsPay = (bool)reader["isPay"]
                            });
                        }
                    }
                }

                foreach (var order in list)
                {
                    order.Items = new List<ClotheOrderItem>();
                    using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT ID, ref, size, quantity, price FROM ClothesOrderItem WHERE order_id = {order.Id}";

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                order.Items.Add(new ClotheOrderItem()
                                {
                                    Id = (int)reader["id"],
                                    Reference = (string)reader["ref"],
                                    Size = (string)reader["size"],
                                    Quantity = (int)reader["quantity"],
                                    Price = (int)reader["price"]
                                });
                            }
                        }
                    }
                }
            }

            return list.OrderByDescending(x => x.Email).ThenBy(x => x.Reference).ToList();
        }

        public List<ClotheOrderItem> GetAllItem()
        {
            var list = new List<ClotheOrderItem>();

            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT clothesorderitem.ref, clothesorderitem.size, SUM(clothesorderitem.quantity) AS quantity, SUM(clothesorderitem.price) AS price FROM clothesorderitem JOIN clothesorder ON clothesorderitem.Order_ID = clothesorder.ID WHERE clothesorder.isDelete = 0 GROUP BY clothesorderitem.ref, clothesorderitem.size";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ClotheOrderItem()
                            {
                                Id = 0,
                                Reference = (string)reader["ref"],
                                Size = (string)reader["size"],
                                Quantity = int.Parse(reader["quantity"].ToString()),
                                Price = int.Parse(reader["price"].ToString())
                            });
                        }
                    }
                }
            }

            return list.OrderBy(x => x.Reference).ToList();
        }

        public ClotheOrder InsertOrder(ClotheOrder order)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO ClothesOrder (email, ref, price) VALUES (@EMAIL, @REF, @PRICE); SELECT LAST_INSERT_ID();";

                    cmd.Parameters.Add("@EMAIL", System.Data.DbType.String);
                    cmd.Parameters["@EMAIL"].Value = order.Email;

                    cmd.Parameters.Add("@REF", System.Data.DbType.String);
                    cmd.Parameters["@REF"].Value = order.Reference;

                    cmd.Parameters.Add("@PRICE", System.Data.DbType.Int16);
                    cmd.Parameters["@PRICE"].Value = order.Price;

                    order.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (var item in order.Items)
                {
                    using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                    {
                        cmd.CommandText = $"INSERT INTO ClothesOrderItem (order_id, ref, size, quantity, price) VALUES (@ORDERID, @REF, @SIZE, @QUANTITY, @PRICE); SELECT LAST_INSERT_ID();";

                        cmd.Parameters.Add("@ORDERID", System.Data.DbType.Int16);
                        cmd.Parameters["@ORDERID"].Value = order.Id;

                        cmd.Parameters.Add("@REF", System.Data.DbType.String);
                        cmd.Parameters["@REF"].Value = item.Reference;

                        cmd.Parameters.Add("@SIZE", System.Data.DbType.String);
                        cmd.Parameters["@SIZE"].Value = item.Size;

                        cmd.Parameters.Add("@QUANTITY", System.Data.DbType.Int16);
                        cmd.Parameters["@QUANTITY"].Value = item.Quantity;

                        cmd.Parameters.Add("@PRICE", System.Data.DbType.Int16);
                        cmd.Parameters["@PRICE"].Value = item.Price;

                        item.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                return order;
            }
        }

        public void UpdateOrderIsPay(int id)
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE ClothesOrder SET isPay = 1 WHERE id = {id}";

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete()
        {
            using (var mysqlconnection = new MySqlConnection(this.ConnectionString))
            {
                mysqlconnection.Open();
                using (MySqlCommand cmd = mysqlconnection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE ClothesOrder SET isDelete = 1 WHERE isDelete = 0";

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
