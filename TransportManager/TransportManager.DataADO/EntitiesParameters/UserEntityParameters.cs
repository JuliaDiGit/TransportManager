using System.Data;
using System.Data.SqlClient;

namespace TransportManager.DataADO.EntitiesParameters
{
    public class UserEntityParameters
    {
        public SqlParameter Id { get; } = new SqlParameter
        {
            ParameterName = "@Id",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.InputOutput
        };

        public SqlParameter Login { get; } = new SqlParameter
        {
            ParameterName = "@Login",
            SqlDbType = SqlDbType.VarChar,
            Direction = ParameterDirection.InputOutput
        };

        public SqlParameter Password { get; } = new SqlParameter
        {
            ParameterName = "@Password",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.InputOutput
        };
        
        public SqlParameter Role { get; } = new SqlParameter
        {
            ParameterName = "@Role",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.InputOutput
        };

        public UserEntityParameters(int id,
                                       string login,
                                       string password,
                                       int role)
        {
            Id.Value = id;
            Login.Value = login;
            Password.Value = password;
            Role.Value = Role;
        }
    }
}