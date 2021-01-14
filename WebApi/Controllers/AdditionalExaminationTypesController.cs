using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.controllers
{
    using System.Linq;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AdditionalExaminationTypesController : Controller
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<AdditionalExaminationType> _additionalExaminationTypeRepository;

        public AdditionalExaminationTypesController(
            IRepository<AdditionalExaminationType> additionalExaminationTypeRepository,
            IIdentityRepository identityRepository)
        {
            _additionalExaminationTypeRepository = additionalExaminationTypeRepository;
            _identityRepository = identityRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<AdditionalExaminationType>> Get()
        {
            return Ok(_additionalExaminationTypeRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AdditionalExaminationType>> Get(int id)
        {
            var additionalExaminationType = await _additionalExaminationTypeRepository.Get(id);

            return additionalExaminationType != null ? Ok(additionalExaminationType) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AdditionalExaminationType>> Post(
            [FromBody] AdditionalExaminationType additionalExaminationType)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var createdAdditionalExaminationType =
                await _additionalExaminationTypeRepository.Add(additionalExaminationType, currentUser);

            return CreatedAtAction(nameof(Post), null, createdAdditionalExaminationType);
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