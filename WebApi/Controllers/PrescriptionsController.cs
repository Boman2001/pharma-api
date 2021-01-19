using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Consultations;
using WebApi.Models.Patients;
using WebApi.Models.Prescriptions;
using WebApi.Models.Users;

namespace WebApi.controllers
{
    using System.Linq;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<Prescription> _prescriptionRepository;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IMapper _mapper;

        public PrescriptionsController(IIdentityRepository identityRepository,
            IRepository<Prescription> prescriptionRepository, IRepository<UserInformation> userInformationRepository,
            IRepository<Patient> patientRepository, IRepository<Consultation> consultationRepository, IMapper mapper)
        {
            _identityRepository = identityRepository;
            _prescriptionRepository = prescriptionRepository;
            _userInformationRepository = userInformationRepository;
            _patientRepository = patientRepository;
            _consultationRepository = consultationRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Prescription>>> Get([FromQuery] int? patientId)
        {
            IEnumerable<Prescription> prescriptions;

            if (patientId.HasValue)
            {
                prescriptions = _prescriptionRepository.Get(p => p.PatientId == patientId, new[]
                {
                    "Consultation", "Patient"
                });
            }
            else
            {
                prescriptions = _prescriptionRepository.Get(new[]
                {
                    "Consultation", "Patient"
                });
            }

            var prescriptionsDtos = new List<PrescriptionDto>();

            foreach (var prescription in prescriptions)
            {
                var user = await _identityRepository.GetUserById(prescription.Consultation.DoctorId.ToString());

                if (user == null)
                {
                    return Problem();
                }

                var userInformation = _userInformationRepository
                    .Get(u => u.UserId == prescription.Consultation.DoctorId)
                    .FirstOrDefault();

                if (userInformation == null)
                {
                    return Problem();
                }

                var consultationDto = _mapper.Map<Consultation, ConsultationDto>(prescription.Consultation);
                var patientDto = _mapper.Map<Patient, PatientDto>(prescription.Patient);
                var userDto = _mapper.Map<IdentityUser, UserDto>(user);
                var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);

                _mapper.Map(userInformationDto, userDto);

                var prescriptionDto = _mapper.Map<Prescription, PrescriptionDto>(prescription);

                prescriptionDto.Patient = patientDto;
                prescriptionDto.Consultation = consultationDto;
                prescriptionDto.Consultation.Doctor = userDto;
                prescriptionDto.Consultation.Patient = patientDto;

                prescriptionsDtos.Add(prescriptionDto);
            }

            return Ok(prescriptionsDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Prescription>> Get(int id)
        {
            var prescription = _prescriptionRepository.Get(id, new[]
            {
                "Consultation", "Patient"
            });

            if (prescription == null)
            {
                return NotFound();
            }

            var user = await _identityRepository.GetUserById(prescription.Consultation.DoctorId.ToString());

            if (user == null)
            {
                return Problem();
            }

            var userInformation = _userInformationRepository.Get(u => u.UserId == prescription.Consultation.DoctorId)
                .FirstOrDefault();

            if (userInformation == null)
            {
                return Problem();
            }

            var consultationDto = _mapper.Map<Consultation, ConsultationDto>(prescription.Consultation);
            var patientDto = _mapper.Map<Patient, PatientDto>(prescription.Patient);
            var userDto = _mapper.Map<IdentityUser, UserDto>(user);
            var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);

            _mapper.Map(userInformationDto, userDto);

            var prescriptionDto = _mapper.Map<Prescription, PrescriptionDto>(prescription);

            prescriptionDto.Patient = patientDto;
            prescriptionDto.Consultation = consultationDto;
            prescriptionDto.Consultation.Doctor = userDto;
            prescriptionDto.Consultation.Patient = patientDto;

            return Ok(prescriptionDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Prescription>> Post([FromBody] NewPrescriptionDto newPrescriptionDto)
        {
            var consultation = await _consultationRepository.Get(newPrescriptionDto.ConsultationId);
            var patient = await _patientRepository.Get(newPrescriptionDto.PatientId);

            if (consultation == null)
            {
                return BadRequest("Consult bestaat niet.");
            }

            if (patient == null)
            {
                return BadRequest("Patient bestaat niet.");
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var prescription = _mapper.Map<NewPrescriptionDto, Prescription>(newPrescriptionDto);

            var createdPrescription = await _prescriptionRepository.Add(prescription, currentUser);

            var createdPrescriptionDto = _mapper.Map<Prescription, CreatedPrescriptionDto>(createdPrescription);

            return CreatedAtAction(nameof(Post), null, createdPrescriptionDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] UpdatePrescriptionDto updatePrescriptionDto)
        {
            var consultation = await _consultationRepository.Get(updatePrescriptionDto.ConsultationId);
            var patient = await _patientRepository.Get(updatePrescriptionDto.PatientId);

            if (consultation == null)
            {
                return BadRequest("Consult bestaat niet.");
            }

            if (patient == null)
            {
                return BadRequest("Patient bestaat niet.");
            }

            var prescription = await _prescriptionRepository.Get(id);

            if (prescription == null)
            {
                return NotFound();
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updatePrescriptionDto, prescription);

            prescription.Id = id;

            var updatedPrescription = await _prescriptionRepository.Update(prescription, currentUser);

            var updatedPrescriptionDto = _mapper.Map<Prescription, UpdatedPrescriptionDto>(updatedPrescription);

            return Ok(updatedPrescriptionDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _prescriptionRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}