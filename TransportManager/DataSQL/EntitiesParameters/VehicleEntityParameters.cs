using System.Data;
using System.Data.SqlClient;

namespace DataSQL.EntitiesParameters
{
    public class VehicleEntityParameters
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
        
        public SqlParameter DriverId { get; } = new SqlParameter
        {
            ParameterName = "@DriverId",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.InputOutput
        };
        
        public SqlParameter Model { get; } = new SqlParameter
        {
            ParameterName = "@Model",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.InputOutput
        };
        
        public SqlParameter GovernmentNumber { get; } = new SqlParameter
        {
            ParameterName = "@GovernmentNumber",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.InputOutput
        };

        public VehicleEntityParameters(int id, 
                                          int companyId, 
                                          int? driverId, 
                                          string model, 
                                          string governmentNumber)
        {
            Id.Value = id;
            CompanyId.Value = companyId;
            DriverId.Value = driverId;
            Model.Value = model;
            GovernmentNumber.Value = governmentNumber;
        }
    }
}