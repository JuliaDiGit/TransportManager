using System;
using System.Collections.Generic;
using Models.Validation.Attributes;

namespace Models
{
    [CompanyModelValidation]
    public class CompanyModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? SoftDeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<DriverModel> Drivers { get; set; }
        public List<VehicleModel> Vehicles { get; set; }
    }
}