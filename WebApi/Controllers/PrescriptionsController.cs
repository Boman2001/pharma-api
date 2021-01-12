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
    public class PrescriptionsController : Controller
    {
        private readonly IRepository<Prescription> _prescriptionRepository;

        public PrescriptionsController(IRepository<Prescription> prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<Prescription>> Get()
        {
            return Ok(_prescriptionRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Prescription>> Get(int id)
        {
            var prescription = await _prescriptionRepository.Get(id);

            return prescription != null ? Ok(prescription) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Prescription>> Post([FromBody] Prescription prescription)
        {
            var createdPrescription = await _prescriptionRepository.Add(prescription);

            return CreatedAtAction(nameof(Post), null, createdPrescription);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] Prescription prescription)
        {
            var updatedPrescription = await _prescriptionRepository.Update(prescription);

            return Ok(updatedPrescription);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            await _prescriptionRepository.Delete(id);

            return NoContent();
        }
    }
}