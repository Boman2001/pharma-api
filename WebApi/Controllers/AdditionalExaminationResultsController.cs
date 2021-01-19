using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.AdditionalExaminationResults;

namespace WebApi.Controllers
{
    using System.Linq;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AdditionalExaminationResultsController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<AdditionalExaminationResult> _additionalExaminationResultRepository;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<AdditionalExaminationType> _additionalExaminationTypeRepository;
        private readonly IMapper _mapper;

        public AdditionalExaminationResultsController(IIdentityRepository identityRepository,
            IRepository<AdditionalExaminationResult> additionalExaminationResultRepository,
            IRepository<Consultation> consultationRepository, IRepository<Patient> patientRepository,
            IRepository<AdditionalExaminationType> additionalExaminationTypeRepository, IMapper mapper)
        {
            _identityRepository = identityRepository;
            _additionalExaminationResultRepository = additionalExaminationResultRepository;
            _consultationRepository = consultationRepository;
            _patientRepository = patientRepository;
            _additionalExaminationTypeRepository = additionalExaminationTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<AdditionalExaminationResult>> Get([FromQuery] int? patientId)
        {
            IEnumerable<AdditionalExaminationResult> additionalExaminationResults;

            if (patientId.HasValue)
            {
                additionalExaminationResults =
                    _additionalExaminationResultRepository.Get(j => j.PatientId == patientId);
            }
            else
            {
                additionalExaminationResults = _additionalExaminationResultRepository.Get();
            }

            var additionalExaminationTypeDtos = additionalExaminationResults
                .Select(additionalExaminationResult =>
                    _mapper.Map<AdditionalExaminationResult, AdditionalExaminationResultDto>(
                        additionalExaminationResult))
                .ToList();

            return Ok(additionalExaminationTypeDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AdditionalExaminationResult>> Get(int id)
        {
            var additionalExaminationResult = await _additionalExaminationResultRepository.Get(id);

            if (additionalExaminationResult == null)
            {
                return NotFound();
            }

            var additionalExaminationTypeDto =
                _mapper.Map<AdditionalExaminationResult, AdditionalExaminationResultDto>(additionalExaminationResult);

            return Ok(additionalExaminationTypeDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AdditionalExaminationResult>> Post(
            [FromBody] CreateAdditionalExaminationResultDto createAdditionalExaminationResultDto)
        {
            var consultation = await _consultationRepository.Get(createAdditionalExaminationResultDto.ConsultationId);

            if (consultation == null)
            {
                return BadRequest("Consult bestaat niet.");
            }

            var patient = await _patientRepository.Get(createAdditionalExaminationResultDto.PatientId);

            if (patient == null)
            {
                return BadRequest("Patiënt bestaat niet.");
            }

            var additionalExaminationType =
                await _additionalExaminationTypeRepository.Get(createAdditionalExaminationResultDto
                    .AdditionalExaminationTypeId);

            if (additionalExaminationType == null)
            {
                return BadRequest("Aanvullend onderzoek type bestaat niet.");
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var additionalExaminationResult =
                _mapper.Map<CreateAdditionalExaminationResultDto, AdditionalExaminationResult>(
                    createAdditionalExaminationResultDto);

            var createdAdditionalExaminationResult =
                await _additionalExaminationResultRepository.Add(additionalExaminationResult, currentUser);

            var createdPrescriptionDto =
                _mapper.Map<AdditionalExaminationResult, AdditionalExaminationResultDto>(
                    createdAdditionalExaminationResult);

            return CreatedAtAction(nameof(Post), createdPrescriptionDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id,
            [FromBody] UpdateAdditionalExaminationResultDto updateAdditionalExaminationResultDto)
        {
            var consultation = await _consultationRepository.Get(updateAdditionalExaminationResultDto.ConsultationId);

            if (consultation == null)
            {
                return BadRequest("Consult bestaat niet.");
            }

            var patient = await _patientRepository.Get(updateAdditionalExaminationResultDto.PatientId);

            if (patient == null)
            {
                return BadRequest("Patiënt bestaat niet.");
            }

            var additionalExaminationResult = await _additionalExaminationResultRepository.Get(id);

            if (additionalExaminationResult == null)
            {
                return BadRequest("Aanvullend onderzoek type bestaat niet.");
            }

            var additionalExaminationType =
                await _additionalExaminationTypeRepository.Get(updateAdditionalExaminationResultDto
                    .AdditionalExaminationTypeId);

            if (additionalExaminationType == null)
            {
                return NotFound();
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updateAdditionalExaminationResultDto, additionalExaminationResult);

            additionalExaminationResult.Id = id;

            var updatedAdditionalExaminationResult =
                await _additionalExaminationResultRepository.Update(additionalExaminationResult, currentUser);

            var updatedAdditionalExaminationType =
                _mapper.Map<AdditionalExaminationResult, AdditionalExaminationResultDto>(
                    updatedAdditionalExaminationResult);

            return Ok(updatedAdditionalExaminationType);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _additionalExaminationResultRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}