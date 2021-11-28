using System;
using TransportManager.Common.Enums;
using TransportManager.Models.Validation.Attributes;

namespace TransportManager.Models
{
    [UserModelValidation]
    public class UserModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? SoftDeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}