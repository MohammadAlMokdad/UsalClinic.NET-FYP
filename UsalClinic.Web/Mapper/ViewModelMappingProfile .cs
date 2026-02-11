using AutoMapper;
using UsalClinic.Application.Models;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.MappingProfiles
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<AppointmentDto, AppointmentViewModel>().ReverseMap();

            
            CreateMap<DepartmentDto, DepartmentViewModel>().ReverseMap();

            CreateMap<DoctorDepartmentDto, DoctorDepartmentViewModel>().ReverseMap();

            CreateMap<DoctorDto, DoctorViewModel>().ReverseMap();

            CreateMap<FAQEntryDto, FAQEntryViewModel>().ReverseMap();

            CreateMap<MedicalRecordDto, MedicalRecordViewModel>().ReverseMap();

            CreateMap<PatientDto, PatientViewModel>().ReverseMap();

            CreateMap<PrescriptionDto, PrescriptionViewModel>().ReverseMap();

            CreateMap<RoomDto, RoomViewModel>().ReverseMap();

            CreateMap<NurseDto, NurseViewModel>().ReverseMap();

            CreateMap<ShiftDto, ShiftViewModel>()
                .ForMember(dest => dest.DaysOfWeek, opt => opt.MapFrom(src => src.DaysOfWeek));
            CreateMap<ShiftViewModel, ShiftDto>()
                .ForMember(dest => dest.DaysOfWeek, opt => opt.MapFrom(src => src.DaysOfWeek));

            CreateMap<PatientRequestDto, PatientRequestViewModel>().ReverseMap();

        }
    }
}
