using AutoMapper;
using Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using WebApi.Models.AdditionalExaminationResults;
using WebApi.Models.AdditionalExaminationTypes;
using WebApi.Models.Authentication;
using WebApi.Models.Episodes;
using WebApi.Models.Intolerances;
using WebApi.Models.ExaminationTypes;
using WebApi.Models.IcpcCodes;
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
            CreateMap<ConsultationDto, Consultation>();
            CreateMap<PrescriptionDto, Prescription>();
            CreateMap<Prescription, CreatedPrescriptionDto>();
            CreateMap<UpdatePrescriptionDto, Prescription>();
            CreateMap<Prescription, UpdatePrescriptionDto>();
            CreateMap<Prescription, UpdatedPrescriptionDto>();
            CreateMap<AdditionalExaminationType, BaseAdditionalExaminationTypeDto>();
            CreateMap<BaseAdditionalExaminationTypeDto, AdditionalExaminationType>();
            CreateMap<AdditionalExaminationType, AdditionalExaminationTypeDto>();
            CreateMap<AdditionalExaminationResult, AdditionalExaminationResultDto>();
            CreateMap<AdditionalExaminationResultDto, AdditionalExaminationResult>();
            CreateMap<AdditionalExaminationResultDto, AdditionalExaminationResult>();
            CreateMap<AdditionalExaminationResult, AdditionalExaminationResultDto>();
            CreateMap<Episode, EpisodeDto>();
            CreateMap<EpisodeDto, Episode>();
            CreateMap<BasePrescriptionDto, Prescription>();
            CreateMap<BaseEpisodeDto, Episode>();
            CreateMap<BaseConsultationDto, Consultation>();
            CreateMap<BaseAdditionalExaminationResultDto, AdditionalExaminationResult>();
            CreateMap<BaseIntoleranceDto, Intolerance>();
            CreateMap<Intolerance, CreatedIntoleranceDto>();
            CreateMap<Intolerance, UpdatedIntoleranceDto>();
            CreateMap<Intolerance, IntoleranceDto>();
            CreateMap<BaseExaminationTypeDto, ExaminationType>();
            CreateMap<ExaminationTypeDto, ExaminationType>();
            CreateMap<ExaminationType, ExaminationTypeDto>();
            CreateMap<BaseIcpcCodeDto, IcpcCode>();
            CreateMap<IcpcCodeDto, IcpcCode>();
            CreateMap<IcpcCode, IcpcCodeDto>();
        }
    }
}