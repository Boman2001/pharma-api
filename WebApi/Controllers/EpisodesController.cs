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
    public class EpisodeController : Controller
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<Episode> _episodeRepository;

        public EpisodeController(IRepository<Episode> episodeRepository, IIdentityRepository identityRepository)
        {
            _episodeRepository = episodeRepository;
            _identityRepository = identityRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<Episode>> Get()
        {
            return Ok(_episodeRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Episode>> Get(int id)
        {
            var episode = await _episodeRepository.Get(id);

            return episode != null ? Ok(episode) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Episode>> Post([FromBody] Episode episode)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var createdEpisode = await _episodeRepository.Add(episode, currentUser);

            return CreatedAtAction(nameof(Post), null, createdEpisode);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] Episode episode)
        {
            episode.Id = id;

            var updatedEpisode = await _episodeRepository.Update(episode);

            return Ok(updatedEpisode);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            await _episodeRepository.Delete(id);

            return NoContent();
        }
    }
}