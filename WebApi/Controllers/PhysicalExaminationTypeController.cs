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
    public class PhysicalExaminationTypesController : Controller
    {
        private readonly IRepository<PhysicalExaminationType> _physicalExaminationTypeRepository;

        public PhysicalExaminationTypesController(
            IRepository<PhysicalExaminationType> physicalExaminationTypeRepository)
        {
            _physicalExaminationTypeRepository = physicalExaminationTypeRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<PhysicalExaminationType>> Get()
        {
            return Ok(_physicalExaminationTypeRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PhysicalExaminationType>> Get(int id)
        {
            var physicalExaminationType = await _physicalExaminationTypeRepository.Get(id);

            return physicalExaminationType != null ? Ok(physicalExaminationType) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PhysicalExaminationType>> Post(
            [FromBody] PhysicalExaminationType physicalExaminationType)
        {
            var createdPhysicalExaminationType = await _physicalExaminationTypeRepository.Add(physicalExaminationType);

            return CreatedAtAction(nameof(Post), null, createdPhysicalExaminationType);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] PhysicalExaminationType physicalExaminationType)
        {
            var updatedPhysicalExaminationType =
                await _physicalExaminationTypeRepository.Update(physicalExaminationType);

            return Ok(updatedPhysicalExaminationType);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            await _physicalExaminationTypeRepository.Delete(id);

            return NoContent();
        }
    }
}