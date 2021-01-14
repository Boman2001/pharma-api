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
    public class PrescriptionsController : Controller
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<Prescription> _prescriptionRepository;

        public PrescriptionsController(IRepository<Prescription> prescriptionRepository,
            IIdentityRepository identityRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _identityRepository = identityRepository;
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
            var userId = User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var createdPrescription = await _prescriptionRepository.Add(prescription, currentUser);

            return CreatedAtAction(nameof(Post), null, createdPrescription);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] Prescription prescription)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            prescription.Id = id;

            var updatedPrescription = await _prescriptionRepository.Update(prescription, currentUser);

            return Ok(updatedPrescription);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _prescriptionRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}