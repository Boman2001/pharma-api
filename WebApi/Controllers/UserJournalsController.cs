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
    public class UserJournalsController : Controller
    {
        private readonly IRepository<UserJournal> _userJournalRepository;

        public UserJournalsController(IRepository<UserJournal> userJournalRepository)
        {
            _userJournalRepository = userJournalRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<UserJournal>> Get()
        {
            return Ok(_userJournalRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserJournal>> Get(int id)
        {
            var userJournal = await _userJournalRepository.Get(id);

            return userJournal != null ? Ok(userJournal) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserJournal>> Post([FromBody] UserJournal userJournal)
        {
            var createdUserJournal = await _userJournalRepository.Add(userJournal);

            return CreatedAtAction(nameof(Post), null, createdUserJournal);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] UserJournal userJournal)
        {
            var updatedUserJournal = await _userJournalRepository.Update(userJournal);

            return Ok(updatedUserJournal);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            await _userJournalRepository.Delete(id);

            return NoContent();
        }
    }
}