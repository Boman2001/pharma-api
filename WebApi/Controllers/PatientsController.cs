using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Helpers;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApi.controllers
{
    using AutoMapper;
    using Models.Patients;
    using System.Linq;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PatientsController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly PatientHelper _patientHelper;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IMapper _mapper;

        public PatientsController(IRepository<Patient> patientRepository, IConfiguration config,
            IIdentityRepository identityRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _identityRepository = identityRepository;
            _mapper = mapper;
            _patientHelper = new PatientHelper(config);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<Patient>> Get()
        {
            return Ok(_patientRepository.Get());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Patient>> Get(int id)
        {
            var patient = await _patientRepository.Get(id);

            return patient != null ? (ActionResult<Patient>)Ok(patient) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Patient>> Post([FromBody] PatientDto patientDto)
        {
            patientDto.Id = 0;

            Patient createdPatient;

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            try
            {
                var patient = _mapper.Map<PatientDto, Patient>(patientDto);
                
                createdPatient = await _patientHelper.AddLatLongToPatient(patient);
                createdPatient = await _patientRepository.Add(createdPatient, currentUser);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });
            }

            return CreatedAtAction(nameof(Post), null, createdPatient);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] Patient patient)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            patient.Id = id;

            var updatedPatient = await _patientRepository.Update(patient, currentUser);

            return Ok(updatedPatient);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            await _patientRepository.Delete(id, currentUser);

            return NoContent();
        }
    }
}