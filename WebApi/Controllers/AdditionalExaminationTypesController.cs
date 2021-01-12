using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
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
    public class AdditionalExaminationTypeController : Controller
    {
        private readonly IRepository<AdditionalExaminationType> _additionalExaminationTypeRepository;

        public AdditionalExaminationTypeController(IRepository<AdditionalExaminationType> additionalExaminationTypeRepository)
        {
            _additionalExaminationTypeRepository = additionalExaminationTypeRepository;
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
        public async Task<ActionResult<AdditionalExaminationType>> Post([FromBody] AdditionalExaminationType additionalExaminationType)
        {
            var createdAdditionalExaminationType = await _additionalExaminationTypeRepository.Add(additionalExaminationType);

            return CreatedAtAction(nameof(Post), null, createdAdditionalExaminationType);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] AdditionalExaminationType additionalExaminationType)
        {
            var updatedAdditionalExaminationType = await _additionalExaminationTypeRepository.Update(additionalExaminationType);

            return Ok(updatedAdditionalExaminationType);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            await _additionalExaminationTypeRepository.Delete(id);

            return NoContent();
        }
    }
}