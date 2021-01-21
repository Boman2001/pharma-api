using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.IcpcCodes;
using System.Linq;
using System.Security.Claims;


namespace WebApi.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class IcpcCodesController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRepository<IcpcCode> _icpcCodeRepository;
        private readonly IMapper _mapper;

        public IcpcCodesController(IRepository<IcpcCode> icpcCodeRepository, IIdentityRepository identityRepository,
            IMapper mapper)
        {
            _icpcCodeRepository = icpcCodeRepository;
            _identityRepository = identityRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<IcpcCode>> Get()
        {
            var icpcCodes = _icpcCodeRepository.Get();

            var icpcCodeDtos = icpcCodes
                .Select(icpcCode =>
                    _mapper.Map<IcpcCode, IcpcCodeDto>(icpcCode))
                .ToList();

            return Ok(icpcCodeDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IcpcCode>> Get(int id)
        {
            var icpcCode = await _icpcCodeRepository.Get(id);

            if (icpcCode == null)
            {
                return NotFound();
            }

            var icpcCodeDto = _mapper.Map<IcpcCode, IcpcCodeDto>(icpcCode);

            return Ok(icpcCodeDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IcpcCode>> Post([FromBody] BaseIcpcCodeDto baseIcpcCodeDto)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var icpcCode = _mapper.Map<BaseIcpcCodeDto, IcpcCode>(baseIcpcCodeDto);

            var createdIcpcCode = await _icpcCodeRepository.Add(icpcCode, currentUser);

            var createdIcpcCodeDto = _mapper.Map<IcpcCode, IcpcCodeDto>(createdIcpcCode);

            return CreatedAtAction(nameof(Post), createdIcpcCodeDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] IcpcCodeDto updateIcpcCodeDto)
        {
            var icpcCode = await _icpcCodeRepository.Get(id);

            if (icpcCode == null)
            {
                return NotFound();
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updateIcpcCodeDto, icpcCode);

            icpcCode.Id = id;

            var updatedAdditionalExaminationType = await _icpcCodeRepository.Update(icpcCode, currentUser);

            var icpcCodeDto = _mapper.Map<IcpcCode, IcpcCodeDto>(updatedAdditionalExaminationType);

            return Ok(icpcCodeDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _icpcCodeRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}