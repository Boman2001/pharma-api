using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Patients;

namespace WebApi.controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Models.Consultations;
    using Models.Users;
    using System.Linq;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ConsultationsController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IMapper _mapper;

        public ConsultationsController(IRepository<Consultation> consultationRepository,
            IIdentityRepository identityRepository, IRepository<UserInformation> userInformationRepository,
            IMapper mapper)
        {
            _consultationRepository = consultationRepository;
            _identityRepository = identityRepository;
            _userInformationRepository = userInformationRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Consultation>>> Get()
        {
            var consultations = _consultationRepository.Get(new[]
            {
                "Patient"
            });
            
            var consultationsDtos = new List<ConsultationDto>();

            foreach (var consultation in consultations)
            {
                var user = await _identityRepository.GetUserById(consultation.DoctorId.ToString());

                if (user == null)
                {
                    return Problem();
                }

                var userInformation = _userInformationRepository.Get(u => u.UserId == consultation.DoctorId)
                    .FirstOrDefault();

                if (userInformation == null)
                {
                    return Problem();
                }

                var patientDto = _mapper.Map<Patient, PatientDto>(consultation.Patient);
                var userDto = _mapper.Map<IdentityUser, UserDto>(user);
                var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);

                _mapper.Map(userInformationDto, userDto);

                var consultationDto = _mapper.Map<Consultation, ConsultationDto>(consultation);

                consultationDto.Doctor = userDto;
                consultationDto.Patient = patientDto;

                consultationsDtos.Add(consultationDto);
            }

            return Ok(consultationsDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Consultation>> Get(int id)
        {
            var consultation = _consultationRepository.Get(id, new[]
            {
                "Patient"
            });

            if (consultation == null)
            {
                return NotFound();
            }

            var consultationDto = _mapper.Map<Consultation, ConsultationDto>(consultation);

            var user = await _identityRepository.GetUserById(consultation.DoctorId.ToString());

            if (user == null)
            {
                return Problem();
            }

            var userInformation = _userInformationRepository.Get(u => u.UserId.ToString() == user.Id)
                .FirstOrDefault();

            if (userInformation == null)
            {
                return Problem();
            }

            var patientDto = _mapper.Map<Patient, PatientDto>(consultation.Patient);
            var userDto = _mapper.Map<IdentityUser, UserDto>(user);
            var userInformationDto = _mapper.Map<UserInformation, UserInformationDto>(userInformation);

            _mapper.Map(userInformationDto, userDto);

            consultationDto.Doctor = userDto;
            consultationDto.Patient = patientDto;

            return Ok(consultationDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Consultation>> Post([FromBody] BaseConsultationDto baseConsultationDto)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var consultation = _mapper.Map<BaseConsultationDto, Consultation>(baseConsultationDto);

            var createdConsultation = await _consultationRepository.Add(consultation, currentUser);

            var createdConsultationDto = _mapper.Map<Consultation, CreatedConsultationDto>(createdConsultation);

            return CreatedAtAction(nameof(Post), null, createdConsultationDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateConsultationDto updateConsultationDto)
        {
            var consultation = await _consultationRepository.Get(id);

            if (consultation == null)
            {
                return NotFound();
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updateConsultationDto,consultation);

            consultation.Id = id;

            var updatedConsultation = await _consultationRepository.Update(consultation, currentUser);

            var updatedConsultationDto = _mapper.Map<Consultation, UpdateConsultationDto>(updatedConsultation);

            return Ok(updatedConsultationDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _consultationRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}