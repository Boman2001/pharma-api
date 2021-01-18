using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public AdditionalExaminationResultsController(
            IRepository<AdditionalExaminationResult> additionalExaminationResultRepository,
            IIdentityRepository identityRepository)
        {
            _additionalExaminationResultRepository = additionalExaminationResultRepository;
            _identityRepository = identityRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<AdditionalExaminationResult>> Get()
        {
            return Ok(_additionalExaminationResultRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AdditionalExaminationResult>> Get(int id)
        {
            var additionalExaminationResult = await _additionalExaminationResultRepository.Get(id);

            return additionalExaminationResult != null ? Ok(additionalExaminationResult) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AdditionalExaminationResult>> Post([FromBody] AdditionalExaminationResult additionalExaminationResult)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var createdAdditionalExaminationResult =
                await _additionalExaminationResultRepository.Add(additionalExaminationResult, currentUser);

            return CreatedAtAction(nameof(Post), null, createdAdditionalExaminationResult);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] AdditionalExaminationResult additionalExaminationResult)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            additionalExaminationResult.Id = id;

            var updatedAdditionalExaminationResult =
                await _additionalExaminationResultRepository.Update(additionalExaminationResult,currentUser);

            return Ok(updatedAdditionalExaminationResult);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _additionalExaminationResultRepository.Delete(id,currentUser);

            return NoContent();
        }
    }
}