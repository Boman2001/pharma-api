using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.AdditionalExaminationTypes;

namespace WebApi.controllers
{
    using System.Linq;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AdditionalExaminationTypesController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<AdditionalExaminationType> _additionalExaminationTypeRepository;
        private readonly IMapper _mapper;

        public AdditionalExaminationTypesController(IIdentityRepository identityRepository,
            IRepository<AdditionalExaminationType> additionalExaminationTypeRepository, IMapper mapper)
        {
            _identityRepository = identityRepository;
            _additionalExaminationTypeRepository = additionalExaminationTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<AdditionalExaminationType>> Get()
        {
            var additionalExaminationTypes = _additionalExaminationTypeRepository.Get();

            var additionalExaminationTypeDtos = additionalExaminationTypes
                .Select(additionalExaminationType =>
                    _mapper.Map<AdditionalExaminationType, AdditionalExaminationTypeDto>(additionalExaminationType))
                .ToList();

            return Ok(additionalExaminationTypeDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AdditionalExaminationType>> Get(int id)
        {
            var additionalExaminationType = await _additionalExaminationTypeRepository.Get(id);

            if (additionalExaminationType == null)
            {
                return NotFound();
            }

            var additionalExaminationTypeDto =
                _mapper.Map<AdditionalExaminationType, AdditionalExaminationTypeDto>(additionalExaminationType);

            return Ok(additionalExaminationTypeDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AdditionalExaminationType>> Post(
            [FromBody] BaseAdditionalExaminationTypeDto baseAdditionalExaminationTypeDto)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var additionalExaminationType = _mapper.Map<BaseAdditionalExaminationTypeDto, AdditionalExaminationType>(baseAdditionalExaminationTypeDto);

            var createdAdditionalExaminationType =
                await _additionalExaminationTypeRepository.Add(additionalExaminationType, currentUser);

            var createdPrescriptionDto = _mapper.Map<AdditionalExaminationType, CreatedAdditionalExaminationTypeDto>(createdAdditionalExaminationType);

            return CreatedAtAction(nameof(Post), createdPrescriptionDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] AdditionalExaminationType additionalExaminationType)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            additionalExaminationType.Id = id;

            var updatedAdditionalExaminationType =
                await _additionalExaminationTypeRepository.Update(additionalExaminationType, currentUser);

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

            await _additionalExaminationTypeRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}