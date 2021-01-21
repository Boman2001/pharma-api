using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Episodes;
using WebApi.Models.IcpcCodes;
using System.Linq;
using System.Security.Claims;


namespace WebApi.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class EpisodesController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<Episode> _episodeRepository;
        private readonly IRepository<Consultation> _consultationRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<IcpcCode> _icpcCodeRepository;
        private readonly IMapper _mapper;

        public EpisodesController(IIdentityRepository identityRepository, IRepository<Episode> episodeRepository,
            IRepository<Consultation> consultationRepository, IRepository<Patient> patientRepository,
            IRepository<IcpcCode> icpcCodeRepository, IMapper mapper)
        {
            _identityRepository = identityRepository;
            _episodeRepository = episodeRepository;
            _consultationRepository = consultationRepository;
            _patientRepository = patientRepository;
            _icpcCodeRepository = icpcCodeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<Episode>> Get([FromQuery] int? patientId, [FromQuery] DateTime? consultDate,
            [FromQuery] bool expired)
        {
            IEnumerable<Episode> episodes;

            if (patientId.HasValue && consultDate.HasValue && expired)
            {
                episodes = _episodeRepository.Get(e =>
                        e.PatientId == patientId.Value &&
                        e.EndDate.Value.Date < consultDate.Value.Date,
                    new[]
                    {
                        "IcpcCode"
                    }
                );
            }
            else if (patientId.HasValue && consultDate.HasValue)
            {
                episodes = _episodeRepository.Get(e =>
                        e.PatientId == patientId.Value &&
                        e.StartDate.Date <= consultDate.Value.Date &&
                        (e.EndDate.Value.Date >= consultDate.Value.Date || e.EndDate == null),
                    new[]
                    {
                        "IcpcCode"
                    }
                );
            }
            else if (patientId.HasValue)
            {
                episodes = _episodeRepository.Get(e => e.PatientId == patientId.Value,
                    new[]
                    {
                        "IcpcCode"
                    });
            }
            else if (consultDate.HasValue)
            {
                episodes = _episodeRepository.Get(
                    e => e.EndDate.Value.Date <= consultDate.Value.Date || e.EndDate == null,
                    new[]
                    {
                        "IcpcCode"
                    });
            }
            else
            {
                episodes = _episodeRepository.Get(new[]
                {
                    "IcpcCode"
                });
            }

            var episodeDtos = new List<EpisodeDto>();

            foreach (var episode in episodes)
            {
                var episodeDto = _mapper.Map<Episode, EpisodeDto>(episode);
                var icpcCodeDto = _mapper.Map<IcpcCode, IcpcCodeDto>(episode.IcpcCode);

                episodeDto.IcpcCode = icpcCodeDto;

                episodeDtos.Add(episodeDto);
            }

            return Ok(episodeDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<Episode> Get(int id)
        {
            var episode = _episodeRepository.Get(id, new[]
            {
                "IcpcCode"
            });

            if (episode == null)
            {
                return NotFound();
            }

            var episodeDto = _mapper.Map<Episode, EpisodeDto>(episode);
            var icpcCodeDto = _mapper.Map<IcpcCode, IcpcCodeDto>(episode.IcpcCode);

            episodeDto.IcpcCode = icpcCodeDto;

            return Ok(episodeDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Episode>> Post([FromBody] BaseEpisodeDto createEpisodeDto)
        {
            if (createEpisodeDto.ConsultationId != null)
            {
                var consultation = await _consultationRepository.Get(createEpisodeDto.ConsultationId.Value);

                if (consultation == null)
                {
                    return BadRequest("Consult bestaat niet.");
                }
            }

            if (createEpisodeDto.PatientId != null)
            {
                var patient = await _patientRepository.Get(createEpisodeDto.PatientId.Value);

                if (patient == null)
                {
                    return BadRequest("Patiënt bestaat niet.");
                }
            }

            if (createEpisodeDto.IcpcCodeId != null)
            {
                var icpcCode = await _icpcCodeRepository.Get(createEpisodeDto.IcpcCodeId.Value);

                if (icpcCode == null)
                {
                    return BadRequest("ICPC Code bestaat niet.");
                }
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var episode = _mapper.Map<BaseEpisodeDto, Episode>(createEpisodeDto);

            var createdEpisode = await _episodeRepository.Add(episode, currentUser);

            var createdEpisodeDto = _mapper.Map<Episode, EpisodeDto>(createdEpisode);

            return CreatedAtAction(nameof(Post), createdEpisodeDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] EpisodeDto updateEpisodeDto)
        {
            var episode = await _episodeRepository.Get(id);

            if (episode == null)
            {
                return NotFound();
            }

            if (updateEpisodeDto.ConsultationId != null)
            {
                var consultation = await _consultationRepository.Get(updateEpisodeDto.ConsultationId.Value);

                if (consultation == null)
                {
                    return BadRequest("Consult bestaat niet.");
                }
            }

            if (updateEpisodeDto.PatientId != null)
            {
                var patient = await _patientRepository.Get(updateEpisodeDto.PatientId.Value);

                if (patient == null)
                {
                    return BadRequest("Patiënt bestaat niet.");
                }
            }

            if (updateEpisodeDto.IcpcCodeId != null)
            {
                var icpcCode = await _icpcCodeRepository.Get(updateEpisodeDto.IcpcCodeId.Value);

                if (icpcCode == null)
                {
                    return BadRequest("ICPC Code bestaat niet.");
                }
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updateEpisodeDto, episode);

            episode.Id = id;

            var updatedEpisode = await _episodeRepository.Update(episode, currentUser);

            var updatedEpisoded = _mapper.Map<Episode, EpisodeDto>(updatedEpisode);

            return Ok(updatedEpisoded);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _episodeRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}