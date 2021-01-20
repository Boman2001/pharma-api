using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.ExaminationTypes;

namespace WebApi.controllers
{
    using System.Linq;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ExaminationTypesController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<ExaminationType> _examinationTypeRepository;
        private readonly IMapper _mapper;

        public ExaminationTypesController(IIdentityRepository identityRepository,
            IRepository<ExaminationType> physicalExaminationTypeRepository, IMapper mapper)
        {
            _identityRepository = identityRepository;
            _examinationTypeRepository = physicalExaminationTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<ExaminationType>> Get()
        {
            var examinationTypes = _examinationTypeRepository.Get();

            var examinationTypeDtos = examinationTypes
                .Select(examinationType =>
                    _mapper.Map<ExaminationType, ExaminationTypeDto>(examinationType))
                .ToList();

            return Ok(examinationTypeDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ExaminationType>> Get(int id)
        {
            var examinationType = await _examinationTypeRepository.Get(id);

            if (examinationType == null)
            {
                return NotFound();
            }

            var examinationTypeDto = _mapper.Map<ExaminationType, ExaminationTypeDto>(examinationType);

            return Ok(examinationTypeDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ExaminationType>> Post([FromBody] BaseExaminationTypeDto baseExaminationTypeDto)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var examinationType = _mapper.Map<BaseExaminationTypeDto, ExaminationType>(baseExaminationTypeDto);

            var createdExaminationType = await _examinationTypeRepository.Add(examinationType, currentUser);

            var createdPrescriptionDto =
                _mapper.Map<ExaminationType, ExaminationTypeDto>(createdExaminationType);

            return CreatedAtAction(nameof(Post), createdPrescriptionDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] ExaminationTypeDto updateExaminationTypeDto)
        {
            var examinationType = await _examinationTypeRepository.Get(id);

            if (examinationType == null)
            {
                return NotFound();
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updateExaminationTypeDto, examinationType);

            examinationType.Id = id;

            var updatedExaminationType = await _examinationTypeRepository.Update(examinationType, currentUser);

            var examinationTypeDto = _mapper.Map<ExaminationType, ExaminationTypeDto>(updatedExaminationType);

            return Ok(examinationTypeDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _examinationTypeRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}