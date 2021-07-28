using System;
using Models.Validation.Attributes;

namespace Models
{
    [VehicleModelValidation]
    public class VehicleModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? SoftDeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int? DriverId { get; set; }
        public string Model { get; set; }
        public string GovernmentNumber { get; set; }
    }
}