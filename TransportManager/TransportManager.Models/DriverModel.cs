using System;
using System.Collections.Generic;
using TransportManager.Models.Validation.Attributes;

namespace TransportManager.Models
{
    [DriverModelValidation]
    public class DriverModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? SoftDeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public List<VehicleModel> Vehicles { get; set; }
    }
}