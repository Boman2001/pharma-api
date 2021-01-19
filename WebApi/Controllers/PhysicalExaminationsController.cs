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
    public class PhysicalExaminationsController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<PhysicalExamination> _physicalExaminationRepository;

        public PhysicalExaminationsController(IRepository<PhysicalExamination> physicalExaminationRepository,
            IIdentityRepository identityRepository)
        {
            _physicalExaminationRepository = physicalExaminationRepository;
            _identityRepository = identityRepository;
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
                physicalExaminations = _physicalExaminationRepository.Get(p => p.PatientId == patientId);
            }
            else
            {
                physicalExaminations = _physicalExaminationRepository.Get();
            }
            
            return Ok(physicalExaminations);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PhysicalExamination>> Get(int id)
        {
            var physicalExamination = await _physicalExaminationRepository.Get(id);

            return physicalExamination != null ? Ok(physicalExamination) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PhysicalExamination>> Post([FromBody] PhysicalExamination physicalExamination)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var createdPhysicalExamination = await _physicalExaminationRepository.Add(physicalExamination, currentUser);

            return CreatedAtAction(nameof(Post), null, createdPhysicalExamination);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] PhysicalExamination physicalExamination)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            physicalExamination.Id = id;

            var updatedPhysicalExamination = await _physicalExaminationRepository.Update(physicalExamination,currentUser);

            return Ok(updatedPhysicalExamination);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _physicalExaminationRepository.Delete(id,currentUser);

            return NoContent();
        }
    }
}