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
using WebApi.Models.PhysicalExaminations;
using WebApi.Models.Prescriptions;
using WebApi.Models.UserJournals;
using WebApi.Models.Users;
using WebApi.Models.Consultations;
using WebApi.Models.Patients;

namespace WebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AdditionalExaminationResult, AdditionalExaminationResultDto>();
            CreateMap<AdditionalExaminationResult, AdditionalExaminationResultDto>();
            CreateMap<AdditionalExaminationResultDto, AdditionalExaminationResult>();
            CreateMap<AdditionalExaminationResultDto, AdditionalExaminationResult>();
            CreateMap<AdditionalExaminationType, AdditionalExaminationTypeDto>();
            CreateMap<AdditionalExaminationType, BaseAdditionalExaminationTypeDto>();
            CreateMap<BaseAdditionalExaminationResultDto, AdditionalExaminationResult>();
            CreateMap<BaseAdditionalExaminationTypeDto, AdditionalExaminationType>();
            CreateMap<BaseConsultationDto, Consultation>();
            CreateMap<BaseConsultationDto, Consultation>();
            CreateMap<BaseEpisodeDto, Episode>();
            CreateMap<BaseExaminationTypeDto, ExaminationType>();
            CreateMap<BaseIcpcCodeDto, IcpcCode>();
            CreateMap<BaseIntoleranceDto, Intolerance>();
            CreateMap<BasePatientDto, Patient>();
            CreateMap<BasePhysicalExaminationDto, PhysicalExamination>();
            CreateMap<BasePrescriptionDto, Prescription>();
            CreateMap<BaseUserJournalDto, UserJournal>();
            CreateMap<Consultation, ConsultationDto>();
            CreateMap<Consultation, CreatedConsultationDto>();
            CreateMap<Consultation, UpdateConsultationDto>();
            CreateMap<ConsultationDto, Consultation>();
            CreateMap<Episode, EpisodeDto>();
            CreateMap<EpisodeDto, Episode>();
            CreateMap<ExaminationType, ExaminationTypeDto>();
            CreateMap<ExaminationTypeDto, ExaminationType>();
            CreateMap<IcpcCode, IcpcCodeDto>();
            CreateMap<IcpcCodeDto, IcpcCode>();
            CreateMap<IdentityUser, UserDto>();
            CreateMap<Intolerance, CreatedIntoleranceDto>();
            CreateMap<Intolerance, IntoleranceDto>();
            CreateMap<Intolerance, UpdatedIntoleranceDto>();
            CreateMap<LoginDto, IdentityUser>();
            CreateMap<Patient, PatientDto>();
            CreateMap<PatientDto, Patient>();
            CreateMap<PhysicalExamination, PhysicalExaminationDto>();
            CreateMap<PhysicalExaminationDto, PhysicalExamination>();
            CreateMap<Prescription, CreatedPrescriptionDto>();
            CreateMap<Prescription, PrescriptionDto>();
            CreateMap<Prescription, UpdatePrescriptionDto>();
            CreateMap<Prescription, UpdatedPrescriptionDto>();
            CreateMap<PrescriptionDto, Prescription>();
            CreateMap<UpdateConsultationDto, Consultation>();
            CreateMap<UpdatePrescriptionDto, Prescription>();
            CreateMap<UserDto, UserDto>();
            CreateMap<UserDto, UserInformation>();
            CreateMap<UserDto, UserInformation>();
            CreateMap<UserDto, UserInformationDto>();
            CreateMap<UserInformation, UserDto>();
            CreateMap<UserInformation, UserInformationDto>();
            CreateMap<UserInformation, UserInformationDto>();
            CreateMap<UserInformationDto, UserDto>();
            CreateMap<UserJournal, UserJournalDto>();
        }
    }
}