using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TransportManager.Entities;

namespace TransportManager.DataEF.EntitiesConfigurations
{
    public class UserEntityConfiguration : EntityTypeConfiguration<UserEntity>
    {
        public UserEntityConfiguration()
        {
            ToTable("users");
            Property(user => user.Login).IsRequired().HasMaxLength(20).IsUnicode(false);
            HasIndex(user => user.Login).IsUnique();
            Property(user => user.Password).IsRequired().HasMaxLength(50);
            Property(user => user.CreatedDate).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}