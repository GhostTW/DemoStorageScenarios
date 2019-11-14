using System.Collections.Generic;
using System.Data;
using System.Linq;
using Demo.Data;
using MySql.Data.MySqlClient;

namespace Demo.Core
{
    public class Repository
    {
        private readonly string _connectionString =
            "server=127.0.0.1;port=3326;user id=root;password=pass.123;database=TestDB;charset=utf8;";

        public IEnumerable<UserEntity> GetUsers()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand(StoredProcedures.SpUserGetUsers, connection))
                {
                    var parameter = new MySqlParameter("@OUT_ReturnValue", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(parameter);
                    var reader = command.ExecuteReader();

                    if (!reader.HasRows) return Enumerable.Empty<UserEntity>();

                    var result = new List<UserEntity>();
                    while (reader.Read())
                    {
                        var id = reader.GetInt32("Id");
                        var code = reader.GetString("Code");
                        var password = reader.GetString("Password");
                        var isActive = reader.GetBoolean("IsActive");
                        var user = new UserEntity {Id = id, Code = code, Password = password, IsActive = isActive};
                        result.Add(user);
                    }

                    return result;
                }
            }
        }

        private struct StoredProcedures
        {
            public const string SpUserCreateUser = "sp_User_CreateUser";
            public const string SpUserGetUsers = "sp_User_GetUsers";
        }
    }
}