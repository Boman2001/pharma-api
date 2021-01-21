using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.UserJournals;
using System.Linq;
using System.Security.Claims;

namespace WebApi.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class UserJournalsController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<UserJournal> _userJournalRepository;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IMapper _mapper;

        public UserJournalsController(IIdentityRepository identityRepository,
            IRepository<UserJournal> userJournalRepository, IRepository<Consultation> consultationRepository,
            IRepository<Patient> patientRepository, IMapper mapper)
        {
            _identityRepository = identityRepository;
            _userJournalRepository = userJournalRepository;
            _consultationRepository = consultationRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<UserJournal>> Get([FromQuery] int? patientId)
        {
            IEnumerable<UserJournal> userJournals;

            if (patientId.HasValue)
            {
                userJournals = _userJournalRepository.Get(e => e.PatientId == patientId.Value);
            }
            else
            {
                userJournals = _userJournalRepository.Get();
            }

            var userJournalDtos = userJournals
                .Select(userJournal => _mapper.Map<UserJournal, UserJournalDto>(userJournal)).ToList();

            return Ok(userJournalDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserJournal>> Get(int id)
        {
            var userJournal = await _userJournalRepository.Get(id);

            if (userJournal == null)
            {
                return NotFound();
            }

            var userJournalDto = _mapper.Map<UserJournal, UserJournalDto>(userJournal);

            return Ok(userJournalDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserJournal>> Post([FromBody] BaseUserJournalDto createUserJournalDto)
        {
            if (createUserJournalDto.ConsultationId != null)
            {
                var consultation = await _consultationRepository.Get(createUserJournalDto.ConsultationId.Value);

                if (consultation == null)
                {
                    return BadRequest("Consult bestaat niet.");
                }
            }

            if (createUserJournalDto.PatientId != null)
            {
                var patient = await _patientRepository.Get(createUserJournalDto.PatientId.Value);

                if (patient == null)
                {
                    return BadRequest("Patiënt bestaat niet.");
                }
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var userJournal = _mapper.Map<BaseUserJournalDto, UserJournal>(createUserJournalDto);

            var createdUserJournal = await _userJournalRepository.Add(userJournal, currentUser);

            var createdUserJournalDto = _mapper.Map<UserJournal, UserJournalDto>(createdUserJournal);

            return CreatedAtAction(nameof(Post), createdUserJournalDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] UserJournalDto updateUserJournalDto)
        {
            var userJournal = await _userJournalRepository.Get(id);

            if (userJournal == null)
            {
                return NotFound();
            }

            if (updateUserJournalDto.ConsultationId != null)
            {
                var consultation = await _consultationRepository.Get(updateUserJournalDto.ConsultationId.Value);

                if (consultation == null)
                {
                    return BadRequest("Consult bestaat niet.");
                }
            }

            if (updateUserJournalDto.PatientId != null)
            {
                var patient = await _patientRepository.Get(updateUserJournalDto.PatientId.Value);

                if (patient == null)
                {
                    return BadRequest("Patiënt bestaat niet.");
                }
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updateUserJournalDto, userJournal);

            userJournal.Id = id;

            var updatedUserJournal = await _userJournalRepository.Update(userJournal, currentUser);

            var updatedUserJournald = _mapper.Map<UserJournal, UserJournalDto>(updatedUserJournal);

            return Ok(updatedUserJournald);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _userJournalRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}