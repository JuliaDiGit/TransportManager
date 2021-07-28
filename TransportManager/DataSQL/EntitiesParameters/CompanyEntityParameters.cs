using System.Data;
using System.Data.SqlClient;

namespace DataSQL.EntitiesParameters
{
    public class CompanyEntityParameters
    {
        public SqlParameter Id { get; } = new SqlParameter
        {
            ParameterName = "@Id",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.InputOutput
        };

        public SqlParameter CompanyId { get; } = new SqlParameter
        {
            ParameterName = "@CompanyId",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.InputOutput
        };
        
        public SqlParameter CompanyName { get; } = new SqlParameter
        {
            ParameterName = "@CompanyName",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.InputOutput
        };

        public CompanyEntityParameters(int id, int companyId, string companyName)
        {
            Id.Value = id;
            CompanyId.Value = companyId;
            CompanyName.Value = companyName;
        }
    }
}