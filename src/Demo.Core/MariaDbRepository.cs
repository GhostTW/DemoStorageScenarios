using System.Collections.Generic;
using System.Data;
using System.Linq;
using Demo.Data;
using MySql.Data.MySqlClient;

namespace Demo.Core
{
    public class MariaDbRepository
    {
        private const string ConnectionString =
            "server=127.0.0.1;port=3326;user id=root;password=pass.123;database=TestDB;charset=utf8;";

        public IEnumerable<UserEntity> GetUsers()
        {
            using (var connection = new MySqlConnection(ConnectionString))
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

        public UserEntity InsertUser(UserEntity user)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand(StoredProcedures.SpUserCreateUser, connection))
                {
                    var parameters = new List<MySqlParameter>();
                    var parameterOut = new MySqlParameter("@OUT_ReturnValue", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var parameterCode = new MySqlParameter("@IN_Code", MySqlDbType.String)
                    {
                        Direction = ParameterDirection.Input,
                        Value =  user.Code
                    };
                    var parameterPassword = new MySqlParameter("@IN_Password", MySqlDbType.String)
                    {
                        Direction = ParameterDirection.Input,
                        Value = user.Password
                    };
                    var parameterIsActive = new MySqlParameter("@IN_IsActive", MySqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Input,
                        Value = user.IsActive
                    };
                    parameters.Add(parameterOut);
                    parameters.Add(parameterCode);
                    parameters.Add(parameterPassword);
                    parameters.Add(parameterIsActive);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());
                    var reader = command.ExecuteReader();
                    
                    if (!reader.HasRows) return null;

                    while (reader.Read())
                    {
                        var id = reader.GetInt32("Id");
                        user.Id = id;

                        break;
                    }

                    return user;
                }
            }
        }

        public void UpdateUser(UserEntity user)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand(StoredProcedures.SpUserUpdateUser, connection))
                {
                    var parameters = new List<MySqlParameter>();
                    var parameterOut = new MySqlParameter("@OUT_ReturnValue", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var parameterId = new MySqlParameter("@IN_Id", MySqlDbType.String)
                    {
                        Direction = ParameterDirection.Input,
                        Value =  user.Id
                    };
                    var parameterPassword = new MySqlParameter("@IN_Password", MySqlDbType.String)
                    {
                        Direction = ParameterDirection.Input,
                        Value =  user.Password
                    };
                    var parameterIsActive = new MySqlParameter("@IN_IsActive", MySqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Input,
                        Value = user.IsActive
                    };
                    parameters.Add(parameterOut);
                    parameters.Add(parameterId);
                    parameters.Add(parameterPassword);
                    parameters.Add(parameterIsActive);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters.ToArray());
                    command.ExecuteNonQuery();
                }
            }
        }
        
        private struct StoredProcedures
        {
            public const string SpUserCreateUser = "sp_User_CreateUser";
            public const string SpUserGetUsers = "sp_User_GetUsers";
            public const string SpUserUpdateUser = "sp_User_UpdateUser";
        }
    }
}