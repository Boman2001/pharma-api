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
using WebApi.Models.Intolerances;
using WebApi.Models.Patients;
using WebApi.Models.Users;
using System.Linq;
using System.Security.Claims;

namespace WebApi.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class IntolerancesController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<Intolerance> _intoleranceRepository;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IMapper _mapper;

        public IntolerancesController(IIdentityRepository identityRepository,
            IRepository<Intolerance> intoleranceRepository, IRepository<UserInformation> userInformationRepository,
            IRepository<Patient> patientRepository, IRepository<Consultation> consultationRepository, IMapper mapper)
        {
            _identityRepository = identityRepository;
            _intoleranceRepository = intoleranceRepository;
            _userInformationRepository = userInformationRepository;
            _patientRepository = patientRepository;
            _consultationRepository = consultationRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Intolerance>>> Get([FromQuery] int? patientId)
        {
            IEnumerable<Intolerance> intolerances;

            if (patientId.HasValue)
            {
                intolerances = _intoleranceRepository.Get(p => p.PatientId == patientId, new[]
                {
                    "Consultation", "Patient"
                });
            }
            else
            {
                intolerances = _intoleranceRepository.Get(new[]
                {
                    "Consultation", "Patient"
                });
            }

            var intolerancesDtos = new List<IntoleranceDto>();

            foreach (var intolerance in intolerances)
            {
                var user = await _identityRepository.GetUserById(intolerance.Consultation.DoctorId.ToString());

                if (user == null)
                {
                    return Problem();
                }

                var userInformation = _userInformationRepository
                    .Get(u => u.UserId == intolerance.Consultation.DoctorId)
                    .FirstOrDefault();

                if (userInformation == null)
                {
                    return Problem();
                }

                var consultationDto = _mapper.Map<Consultation, ConsultationDto>(intolerance.Consultation);
                var patientDto = _mapper.Map<Patient, PatientDto>(intolerance.Patient);
                var userDto = _mapper.Map<IdentityUser, UserDto>(user);
                var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);

                _mapper.Map(userInformationDto, userDto);

                var intoleranceDto = _mapper.Map<Intolerance, IntoleranceDto>(intolerance);

                intoleranceDto.Patient = patientDto;
                intoleranceDto.Consultation = consultationDto;
                intoleranceDto.Consultation.Doctor = userDto;
                intoleranceDto.Consultation.Patient = patientDto;

                intolerancesDtos.Add(intoleranceDto);
            }

            return Ok(intolerancesDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Intolerance>> Get(int id)
        {
            var intolerance = _intoleranceRepository.Get(id, new[]
            {
                "Consultation", "Patient"
            });

            if (intolerance == null)
            {
                return NotFound();
            }

            var user = await _identityRepository.GetUserById(intolerance.Consultation.DoctorId.ToString());

            if (user == null)
            {
                return Problem();
            }

            var userInformation = _userInformationRepository.Get(u => u.UserId == intolerance.Consultation.DoctorId)
                .FirstOrDefault();

            if (userInformation == null)
            {
                return Problem();
            }

            var consultationDto = _mapper.Map<Consultation, ConsultationDto>(intolerance.Consultation);
            var patientDto = _mapper.Map<Patient, PatientDto>(intolerance.Patient);
            var userDto = _mapper.Map<IdentityUser, UserDto>(user);
            var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);

            _mapper.Map(userInformationDto, userDto);

            var intoleranceDto = _mapper.Map<Intolerance, IntoleranceDto>(intolerance);

            intoleranceDto.Patient = patientDto;
            intoleranceDto.Consultation = consultationDto;
            intoleranceDto.Consultation.Doctor = userDto;
            intoleranceDto.Consultation.Patient = patientDto;

            return Ok(intoleranceDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Intolerance>> Post([FromBody] BaseIntoleranceDto createIntoleranceDto)
        {
            if (createIntoleranceDto.ConsultationId != null)
            {
                var consultation = await _consultationRepository.Get(createIntoleranceDto.ConsultationId.Value);

                if (consultation == null)
                {
                    return BadRequest("Consult bestaat niet.");
                }
            }

            if (createIntoleranceDto.PatientId != null)
            {
                var patient = await _patientRepository.Get(createIntoleranceDto.PatientId.Value);

                if (patient == null)
                {
                    return BadRequest("Patient bestaat niet.");
                }
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var intolerance = _mapper.Map<BaseIntoleranceDto, Intolerance>(createIntoleranceDto);

            var createdIntolerance = await _intoleranceRepository.Add(intolerance, currentUser);

            var createdIntoleranceDto = _mapper.Map<Intolerance, CreatedIntoleranceDto>(createdIntolerance);

            return CreatedAtAction(nameof(Post), null, createdIntoleranceDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateIntoleranceDto updateIntoleranceDto)
        {
            var intolerance = await _intoleranceRepository.Get(id);

            if (intolerance == null)
            {
                return NotFound();
            }

            if (updateIntoleranceDto.ConsultationId != null)
            {
                var consultation = await _consultationRepository.Get(updateIntoleranceDto.ConsultationId.Value);

                if (consultation == null)
                {
                    return BadRequest("Consult bestaat niet.");
                }
            }

            if (updateIntoleranceDto.PatientId != null)
            {
                var patient = await _patientRepository.Get(updateIntoleranceDto.PatientId.Value);

                if (patient == null)
                {
                    return BadRequest("Patiënt bestaat niet.");
                }
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updateIntoleranceDto, intolerance);

            intolerance.Id = id;

            var updatedIntolerance = await _intoleranceRepository.Update(intolerance, currentUser);

            var updatedIntoleranceDto = _mapper.Map<Intolerance, UpdatedIntoleranceDto>(updatedIntolerance);

            return Ok(updatedIntoleranceDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _intoleranceRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}