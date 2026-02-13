using Application.DTOs.Auth;
using Application.DTOs.Department;
using Application.DTOs.Employee;
using Application.DTOs.LeaveRequest;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 1. İZİN TALEPLERİ (LeaveRequest -> DetailDto)
            CreateMap<LeaveRequest, LeaveRequestDetailDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.Name)) 
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Department_Name, opt => opt.MapFrom(src => src.Employee.Department != null ? src.Employee.Department.Name : "-"));

            // CreateDto -> Entity (Ana Tanım)
            CreateMap<CreateLeaveRequestDto, LeaveRequest>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => LeaveRequestStatus.Pending));

            // UpdateDto -> Entity
            CreateMap<UpdateLeaveRequestDto, LeaveRequest>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // 3. DEPARTMAN (Department -> Dto)
            CreateMap<Department, DepartmentDto>().ReverseMap();
            // Constructor içine ekle:

            // 3. DEPARTMAN MAPPINGLERİ
            // Veritabanından Okuma (Entity -> Dto)
            CreateMap<Domain.Entities.Department, DepartmentDto>();

            // Ekleme İşlemi (CreateDto -> Entity)
            CreateMap<CreateDepartmentDto, Department>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()); // Service'de atanıyor

            // Güncelleme İşlemi (UpdateDto -> Entity)
            CreateMap<UpdateDepartmentDto, Department>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            // 2. ÇALIŞANLAR (Employee -> Dto)

            // Listeleme için (DepartmentName'i map'liyoruz)
            CreateMap<Employee, EmployeeListDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : "-"));

            // Detay için
            CreateMap<Employee, EmployeeDetailDto>();

            // Update için (UpdateDto -> Entity)
            CreateMap<UpdateEmployeeDto,Employee>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));




        }
    }
}