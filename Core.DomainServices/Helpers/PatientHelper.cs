using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Models;
using Geocoding;
using Geocoding.Google;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;

namespace Core.DomainServices.Helpers
{
    public class PatientHelper
    {
        private IConfiguration config { get; }

        public PatientHelper(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<Patient> AddLatLongToPatient(Patient patient)
        {
            try
            {
                string formattedAddress = patient.HouseNumber + patient.HouseNumberAddon + " " + patient.Street + " " + patient.City +" "+
                                          patient.Country;
                IGeocoder geocoder = new GoogleGeocoder() { ApiKey = config["GoogleApiKey"] };
                var addresses = await geocoder.GeocodeAsync(formattedAddress);
                patient.Latitude = addresses.First().Coordinates.Latitude;
                 patient.Longditude = addresses.First().Coordinates.Longitude;

                return patient;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
