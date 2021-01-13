using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Models;
using Geocoding;
using Geocoding.Google;
using Microsoft.Extensions.Configuration;

namespace Core.DomainServices.Helpers
{
    public class PatientHelper
    {
        private readonly IConfiguration _configuration;

        public PatientHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Patient> AddLatLongToPatient(Patient patient)
        {
            var formattedAddress =
                $"{patient.HouseNumber} {patient.HouseNumberAddon} {patient.Street} {patient.City} {patient.Country}";

            var geocoder = new GoogleGeocoder {ApiKey = _configuration["GoogleApiKey"]};
            var addresses = await geocoder.GeocodeAsync(formattedAddress);

            var enumerable = addresses.ToList();
            patient.Latitude = enumerable.First().Coordinates.Latitude;
            patient.Longditude = enumerable.First().Coordinates.Longitude;

            return patient;
        }
    }
}