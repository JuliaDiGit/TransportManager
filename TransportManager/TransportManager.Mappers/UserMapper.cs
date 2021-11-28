using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Models;

namespace TransportManager.Mappers
{
    public static class UserMapper
    {
        public static UserEntity ToEntity(this User user)
        {
            if (user == null) return null;
            
            return new UserEntity
            {
                Id = user.Id,
                CreatedDate = user.CreatedDate,
                Login = user.Login,
                Password = user.Password,
                Role = user.Role,
                IsDeleted = user.IsDeleted,
                SoftDeletedDate = user.SoftDeletedDate
            };
        }

        public static User ToDomain(this UserModel userModel)
        {
            if (userModel == null) return null;

            return new User
            {
                Id = userModel.Id,
                CreatedDate = userModel.CreatedDate,
                Login = userModel.Login,
                Password = userModel.Password,
                Role = userModel.Role,
                IsDeleted = userModel.IsDeleted,
                SoftDeletedDate = userModel.SoftDeletedDate
            };
        }
        
        public static User ToDomain(this UserEntity userEntity)
        {
            if (userEntity == null) return null;

            return new User
            {
                Id = userEntity.Id,
                CreatedDate = userEntity.CreatedDate,
                Login = userEntity.Login,
                Password = userEntity.Password,
                Role = userEntity.Role,
                IsDeleted = userEntity.IsDeleted,
                SoftDeletedDate = userEntity.SoftDeletedDate
            };
        }

        public static UserModel ToModel(this User user)
        {
            if (user == null) return null;
            
            return new UserModel
            {
                Id = user.Id,
                CreatedDate = user.CreatedDate,
                Login = user.Login,
                Password = user.Password,
                Role = user.Role,
                IsDeleted = user.IsDeleted,
                SoftDeletedDate = user.SoftDeletedDate
            };
        }
    }
}