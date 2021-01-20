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
    public class ExaminationTypesController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<ExaminationType> _physicalExaminationTypeRepository;

        public ExaminationTypesController(
            IRepository<ExaminationType> physicalExaminationTypeRepository, IIdentityRepository identityRepository)
        {
            _physicalExaminationTypeRepository = physicalExaminationTypeRepository;
            _identityRepository = identityRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<ExaminationType>> Get()
        {
            return Ok(_physicalExaminationTypeRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ExaminationType>> Get(int id)
        {
            var physicalExaminationType = await _physicalExaminationTypeRepository.Get(id);

            return physicalExaminationType != null ? Ok(physicalExaminationType) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ExaminationType>> Post([FromBody] ExaminationType examinationType)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var createdPhysicalExaminationType = await _physicalExaminationTypeRepository.Add(examinationType,currentUser);

            return CreatedAtAction(nameof(Post), null, createdPhysicalExaminationType);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] ExaminationType examinationType)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            examinationType.Id = id;

            var updatedPhysicalExaminationType =
                await _physicalExaminationTypeRepository.Update(examinationType,currentUser);

            return Ok(updatedPhysicalExaminationType);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _physicalExaminationTypeRepository.Delete(id,currentUser);

            return NoContent();
        }
    }
}