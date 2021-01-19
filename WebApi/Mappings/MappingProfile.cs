using AutoMapper;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using WebApi.Models.AdditionalExaminationTypes;
using WebApi.Models.Authentication;
using WebApi.Models.Prescriptions;
using WebApi.Models.Users;

namespace WebApi.Mappings
{
    using Models.Consultations;
    using Models.Patients;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityUser, UserDto>();
            CreateMap<UserInformation, UserDto>();
            CreateMap<UserInformation, UserInformationDto>();
            CreateMap<UserInformationDto, UserDto>();
            CreateMap<UserDto, UserInformationDto>();
            CreateMap<UserDto, UserInformation>();
            CreateMap<UserDto, UserDto>();
            CreateMap<UserDto, UserInformation>();
            CreateMap<UserInformation, UserInformationDto>();
            CreateMap<LoginDto, IdentityUser>();
            CreateMap<PatientDto, Patient>();
            CreateMap<Consultation, ConsultationDto>();
            CreateMap<BaseConsultationDto, Consultation>();
            CreateMap<UpdateConsultationDto, Consultation>();
            CreateMap<Consultation, CreatedConsultationDto>();
            CreateMap<Consultation, UpdateConsultationDto>();
            CreateMap<Patient, PatientDto>();
            CreateMap<Prescription, PrescriptionDto>();
            CreateMap<NewConsultationDto, Consultation>();
            CreateMap<NewPrescriptionDto, Prescription>();
            CreateMap<Prescription, CreatedPrescriptionDto>();
            CreateMap<UpdatePrescriptionDto, Prescription>();
            CreateMap<Prescription, UpdatePrescriptionDto>();
            CreateMap<Prescription, UpdatedPrescriptionDto>();
            CreateMap<AdditionalExaminationType, BaseAdditionalExaminationTypeDto>();
            CreateMap<BaseAdditionalExaminationTypeDto, AdditionalExaminationType>();
            CreateMap<AdditionalExaminationType, CreatedAdditionalExaminationTypeDto>();
            CreateMap<AdditionalExaminationType, AdditionalExaminationTypeDto>();
        }
    }
}