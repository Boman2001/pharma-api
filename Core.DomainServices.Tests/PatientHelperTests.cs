using System;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Core.Domain.Models;
using Core.DomainServices.Helpers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Core.DomainServices.Tests
{
    public class PatientHelperTests
    {
        [Trait("Category", "PatientHelper Tests")]
        [Fact]
        public async Task Given_Patient_Sets_Latitude_LongitudeAsync()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var patientHelper = new PatientHelper(config);

            var patient = new Patient
            {
                Name = "Patient",
                Bsn = "Bsn",
                Email = "maartendonkersloot@gmail.com",
                Dob = DateTime.Now,
                Gender = Gender.Male,
                PhoneNumber = "PhoneNumber",
                City = "Hank",
                Street = "Lepelaarstraat",
                HouseNumber = "20",
                HouseNumberAddon = null,
                PostalCode = "4273CV",
                Country = "Netherlands"
            };

            await patientHelper.AddLatLongToPatient(patient);

            Assert.True(patient.Latitude > 0);
            Assert.True(patient.Longitude > 0);
        }

        [Trait("Category", "PatientHelper Tests")]
        [Fact]
        public async Task Given_Patient_Sets_Latitude_LongitudeAsync_Returns_Patient()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var patientHelper = new PatientHelper(config);

            var patient = new Patient
            {
                Name = "Patient",
                Bsn = "Bsn",
                Email = "maartendonkersloot@gmail.com",
                Dob = DateTime.Now,
                Gender = Gender.Male,
                PhoneNumber = "PhoneNumber",
                City = "Hank",
                Street = "Lepelaarstraat",
                HouseNumber = "20",
                HouseNumberAddon = null,
                PostalCode = "4273CV",
                Country = "Netherlands"
            };

            var result = await patientHelper.AddLatLongToPatient(patient);
            
            Assert.Equal(result.Name, patient.Name);
            Assert.True(result.Latitude > 0);
            Assert.True(result.Longitude > 0);
        }
    }
}
