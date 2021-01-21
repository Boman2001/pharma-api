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
            var patients = _patientRepository.Get();

            var patientDtos = patients.Select(patient => _mapper.Map<Patient, PatientDto>(patient)).ToList();

            return Ok(patientDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Patient>> Get(int id)
        {
            var patient = await _patientRepository.Get(id);

            if (patient == null)
            {
                return NotFound();
            }

            var patientDto = _mapper.Map<Patient, PatientDto>(patient);

            return Ok(patientDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Patient>> Post([FromBody] BasePatientDto basePatientDto)
        {
            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            var patient = _mapper.Map<BasePatientDto, Patient>(basePatientDto);

            var createdPatient = await _patientRepository.Add(patient, currentUser);

            var createdPrescriptionDto = _mapper.Map<Patient, PatientDto>(createdPatient);

            return CreatedAtAction(nameof(Post), createdPrescriptionDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put(int id, [FromBody] BasePatientDto updatePatientDto)
        {
            var patient = await _patientRepository.Get(id);

            if (patient == null)
            {
                return NotFound();
            }

            var userId = User.Claims.First(u => u.Type == ClaimTypes.Sid).Value;
            var currentUser = await _identityRepository.GetUserById(userId);

            _mapper.Map(updatePatientDto, patient);

            patient.Id = id;

            var updatedPatient = await _patientRepository.Update(patient, currentUser);

            var patientDto = _mapper.Map<Patient, PatientDto>(updatedPatient);

            return Ok(patientDto);
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