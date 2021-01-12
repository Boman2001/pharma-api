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
    public class IcpcCodeController : Controller
    {
        private readonly IRepository<IcpcCode> _icpcCodeRepository;

        public IcpcCodeController(IRepository<IcpcCode> icpcCodeRepository)
        {
            _icpcCodeRepository = icpcCodeRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<IcpcCode>> Get()
        {
            return Ok(_icpcCodeRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IcpcCode>> Get(int id)
        {
            var icpcCode = await _icpcCodeRepository.Get(id);

            return icpcCode != null ? Ok(icpcCode) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IcpcCode>> Post([FromBody] IcpcCode icpcCode)
        {
            var createdIcpcCode = await _icpcCodeRepository.Add(icpcCode);

            return CreatedAtAction(nameof(Post), null, createdIcpcCode);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] IcpcCode icpcCode)
        {
            var updatedIcpcCode = await _icpcCodeRepository.Update(icpcCode);

            return Ok(updatedIcpcCode);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            await _icpcCodeRepository.Delete(id);

            return NoContent();
        }
    }
}