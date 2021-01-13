using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PhysicalExaminationsController : Controller
    {
        private readonly IRepository<PhysicalExamination> _physicalExaminationRepository;

        public PhysicalExaminationsController(IRepository<PhysicalExamination> physicalExaminationRepository)
        {
            _physicalExaminationRepository = physicalExaminationRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<PhysicalExamination>> Get()
        {
            return Ok(_physicalExaminationRepository.Get());
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
            var createdPhysicalExamination = await _physicalExaminationRepository.Add(physicalExamination);

            return CreatedAtAction(nameof(Post), null, createdPhysicalExamination);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] PhysicalExamination physicalExamination)
        {
            physicalExamination.Id = id;
            
            var updatedPhysicalExamination = await _physicalExaminationRepository.Update(physicalExamination);

            return Ok(updatedPhysicalExamination);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            await _physicalExaminationRepository.Delete(id);

            return NoContent();
        }
    }
}