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
    public class IntolerancesController : Controller
    {
        private readonly IRepository<Intolerance> _intoleranceRepository;

        public IntolerancesController(IRepository<Intolerance> intoleranceRepository)
        {
            _intoleranceRepository = intoleranceRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<Intolerance>> Get()
        {
            return Ok(_intoleranceRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Intolerance>> Get(int id)
        {
            var intolerance = await _intoleranceRepository.Get(id);

            return intolerance != null ? Ok(intolerance) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Intolerance>> Post([FromBody] Intolerance intolerance)
        {
            var createdIntolerance = await _intoleranceRepository.Add(intolerance);

            return CreatedAtAction(nameof(Post), null, createdIntolerance);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] Intolerance intolerance)
        {
            intolerance.Id = id;

            var updatedIntolerance = await _intoleranceRepository.Update(intolerance);

            return Ok(updatedIntolerance);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            await _intoleranceRepository.Delete(id);

            return NoContent();
        }
    }
}