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
    public class ConsultationsController : Controller
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<Consultation> _consultationRepository;

        public ConsultationsController(IRepository<Consultation> consultationRepository, IIdentityRepository identityRepository)
        {
            _consultationRepository = consultationRepository;
            _identityRepository = identityRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<Consultation>> Get()
        {
            return Ok(_consultationRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Consultation>> Get(int id)
        {
            var consultation = await _consultationRepository.Get(id);

            return consultation != null ? Ok(consultation) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Consultation>> Post([FromBody] Consultation consultation)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = await _identityRepository.GetUserById(userId);
            
            var createdConsultation = await _consultationRepository.Add(consultation, currentUser);

            return CreatedAtAction(nameof(Post), null, createdConsultation);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] Consultation consultation)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            consultation.Id = id;

            var updatedConsultation = await _consultationRepository.Update(consultation,currentUser);

            return Ok(updatedConsultation);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _consultationRepository.Delete(id,currentUser);

            return NoContent();
        }
    }
}