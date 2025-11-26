using Application.DTOs.LeaveRequest;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // LeaveRequest -> LeaveRequestDetailDto Dönüşümü
            CreateMap<LeaveRequest, LeaveRequestDetailDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // CreateLeaveRequestDto -> LeaveRequest Dönüşümü (Tersine işlem)
            CreateMap<CreateLeaveRequestDto, LeaveRequest>();
        }
    }
}
