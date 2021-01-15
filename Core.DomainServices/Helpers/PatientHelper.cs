using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Models;
using Geocoding.Google;
using Microsoft.Extensions.Configuration;

namespace Core.DomainServices.Helpers
{
    using System;
    using System.Collections.Generic;

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

            var geocoder = new GoogleGeocoder
            {
                ApiKey = _configuration["GoogleApiKey"]
            };

            try
            {
                var addresses = await geocoder.GeocodeAsync(formattedAddress);
                var enumerable = addresses.ToList();

                patient.Latitude = enumerable.First().Coordinates.Latitude;
                patient.Longitude = enumerable.First().Coordinates.Longitude;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid address.");
            }

            return patient;
        }
    }
}