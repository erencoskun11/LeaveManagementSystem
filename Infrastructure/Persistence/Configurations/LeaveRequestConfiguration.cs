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
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.HasOne(lr=>lr.Employee)
                .WithMany(e=>e.LeaveRequests)
                .HasForeignKey(lr=>lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Property(lr=>lr.RequestComments)
                .HasMaxLength(400)
            .IsRequired(false);





        }
    }
}
