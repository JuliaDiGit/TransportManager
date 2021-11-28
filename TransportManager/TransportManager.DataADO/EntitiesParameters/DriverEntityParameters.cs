using System.Data;
using System.Data.SqlClient;

namespace TransportManager.DataADO.EntitiesParameters
{
    public class DriverEntityParameters
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
        
        public SqlParameter Name { get; } = new SqlParameter
        {
            ParameterName = "@Name",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.InputOutput
        };

        public DriverEntityParameters(int id, int companyId, string name)
        {
            Id.Value = id;
            CompanyId.Value = companyId;
            Name.Value = name;
        }
    }
}