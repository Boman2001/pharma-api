using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.PhysicalExaminations;
using WebApi.Models.ExaminationTypes;
using System.Linq;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PhysicalExaminationsController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<PhysicalExamination> _physicalExaminationRepository;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<ExaminationType> _examinationTypeRepository;
        private readonly IMapper _mapper;

        public PhysicalExaminationsController(IIdentityRepository identityRepository,
            IRepository<PhysicalExamination> physicalExaminationRepository,
            IRepository<Consultation> consultationRepository, IRepository<Patient> patientRepository,
            IRepository<ExaminationType> examinationTypeRepository, IMapper mapper)
        {
            _identityRepository = identityRepository;
            _physicalExaminationRepository = physicalExaminationRepository;
            _consultationRepository = consultationRepository;
            _patientRepository = patientRepository;
            _examinationTypeRepository = examinationTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<PhysicalExamination>> Get([FromQuery] int? patientId)
        {
            IEnumerable<PhysicalExamination> physicalExaminations;

            if (patientId.HasValue)
            {
                physicalExaminations =
                    _physicalExaminationRepository.Get(j => j.PatientId == patientId, new[]
                    {
                        "ExaminationType"
                    });
            }
            else
            {
                physicalExaminations = _physicalExaminationRepository.Get(new[]
                {
                    "ExaminationType"
                });
            }

            var physicalExaminationDtos = new List<PhysicalExaminationDto>();

            foreach (var physicalExamination in physicalExaminations)
            {
                var physicalExaminationDto =
                    _mapper.Map<PhysicalExamination, PhysicalExaminationDto>(physicalExamination);
                var examinationTypeDto =
                    _mapper.Map<ExaminationType, ExaminationTypeDto>(physicalExamination.ExaminationType);

                physicalExaminationDto.ExaminationType = examinationTypeDto;

                physicalExaminationDtos.Add(physicalExaminationDto);
            }

            return Ok(physicalExaminationDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<PhysicalExamination> Get(int id)
        {
            var physicalExamination = _physicalExaminationRepository.Get(id, new[]
            {
                "ExaminationType"
            });

            if (physicalExamination == null)
            {
                return NotFound();
            }

            var examinationTypeDto =
                _mapper.Map<PhysicalExamination, PhysicalExaminationDto>(physicalExamination);

            return Ok(examinationTypeDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PhysicalExamination>> Post(
            [FromBody] BasePhysicalExaminationDto createPhysicalExaminationDto)
        {
            if (createPhysicalExaminationDto.ConsultationId != null)
            {
                var consultation =
                    await _consultationRepository.Get(createPhysicalExaminationDto.ConsultationId.Value);

                if (consultation == null)
                {
                    return BadRequest("Consult bestaat niet.");
                }
            }

            if (createPhysicalExaminationDto.PatientId != null)
            {
                var patient = await _patientRepository.Get(createPhysicalExaminationDto.PatientId.Value);

                if (patient == null)
                {
                    return BadRequest("Patiënt bestaat niet.");
                }
            }

            if (createPhysicalExaminationDto.ExaminationTypeId != null)
            {
                var examinationType =
                    await _examinationTypeRepository.Get(createPhysicalExaminationDto.ExaminationTypeId.Value);

                if (examinationType == null)
                {
                    return BadRequest("Aanvullend onderzoek type bestaat niet.");
                }
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var physicalExamination =
                _mapper.Map<BasePhysicalExaminationDto, PhysicalExamination>(createPhysicalExaminationDto);

            var createdPhysicalExamination = await _physicalExaminationRepository.Add(physicalExamination, currentUser);

            var createdPrescriptionDto =
                _mapper.Map<PhysicalExamination, PhysicalExaminationDto>(createdPhysicalExamination);

            return CreatedAtAction(nameof(Post), createdPrescriptionDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] BasePhysicalExaminationDto updatePhysicalExaminationDto)
        {
            var physicalExamination = await _physicalExaminationRepository.Get(id);

            if (physicalExamination == null)
            {
                return NotFound();
            }

            if (updatePhysicalExaminationDto.ExaminationTypeId != null)
            {
                var examinationType =
                    await _examinationTypeRepository.Get(updatePhysicalExaminationDto.ExaminationTypeId.Value);

                if (examinationType == null)
                {
                    return BadRequest("Aanvullend onderzoek type bestaat niet.");
                }
            }

            if (updatePhysicalExaminationDto.ConsultationId != null)
            {
                var consultation =
                    await _consultationRepository.Get(updatePhysicalExaminationDto.ConsultationId.Value);

                if (consultation == null)
                {
                    return BadRequest("Consult bestaat niet.");
                }
            }

            if (updatePhysicalExaminationDto.PatientId != null)
            {
                var patient = await _patientRepository.Get(updatePhysicalExaminationDto.PatientId.Value);

                if (patient == null)
                {
                    return BadRequest("Patiënt bestaat niet.");
                }
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updatePhysicalExaminationDto, physicalExamination);

            physicalExamination.Id = id;

            var updatedPhysicalExamination =
                await _physicalExaminationRepository.Update(physicalExamination, currentUser);

            var updatedExaminationType =
                _mapper.Map<PhysicalExamination, PhysicalExaminationDto>(updatedPhysicalExamination);

            return Ok(updatedExaminationType);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _physicalExaminationRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}