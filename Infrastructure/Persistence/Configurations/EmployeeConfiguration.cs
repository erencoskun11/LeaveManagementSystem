using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");


            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e=>e.LastName)
                .IsRequired()
                .HasMaxLength(35);

            builder.Property(e=>e.Email)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(e => e.Role)
                .IsRequired();
        }
    }
}
