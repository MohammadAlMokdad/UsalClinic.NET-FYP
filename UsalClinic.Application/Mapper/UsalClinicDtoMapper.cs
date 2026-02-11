using UsalClinic.Application.Models;
using UsalClinic.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using AutoMapper;
using System;


namespace UsalClinic.Application.Mapper
{
    public class UsalClinicDtoMapper : Profile
    {
        public UsalClinicDtoMapper()
        {
            //Appointment Mapping
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.FullName))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.User.FullName))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId));
            CreateMap<AppointmentDto,Appointment>()
                .ForMember(dest => dest.Doctor, opt => opt.Ignore()) 
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Doctor Mapping
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
            CreateMap<DoctorDto, Doctor>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Room Mapping
            CreateMap<Room, RoomDto>();

            CreateMap<RoomDto, Room>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); 


            //Department Mapping
            CreateMap<Department, DepartmentDto>().ReverseMap();

            //Prescription Mapping
            CreateMap<Prescription, PrescriptionDto>()
                .ForMember(dest => dest.patientId, opt => opt.MapFrom(src => src.MedicalRecord.PatientId));
            CreateMap<PrescriptionDto, Prescription>()
                .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Medical Record Mapping
            CreateMap<MedicalRecord, MedicalRecordDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.FullName))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.User.FullName))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId));
            CreateMap<MedicalRecordDto,MedicalRecord>()
                 .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //FAQ Mapping
            CreateMap<FAQEntry,FAQEntryDto>();

            CreateMap<FAQEntryDto, FAQEntry>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            //Patient Mapping
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
            CreateMap<PatientDto, Patient>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Nurse Mapping
            CreateMap<Nurse, NurseDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<NurseDto, Nurse>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Shift Mapping
            CreateMap<Shift, ShiftDto>()
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff.FullName))
                .ForMember(dest => dest.DaysOfWeek, opt => opt.MapFrom(src => src.DaysOfWeek));
            CreateMap<ShiftDto, Shift>()
                .ForMember(dest => dest.DaysOfWeek, opt => opt.MapFrom(src => src.DaysOfWeek));

            CreateMap<PatientRequest, PatientRequestDto>().ReverseMap();

        }
    }
}

