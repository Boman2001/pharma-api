using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using Core.Domain.DataTransferObject;
using AutoMapper;

namespace WebApi.Mappings
{
    public class MappingProfile :Profile{
        public MappingProfile()
        {
            CreateMap<UserInformation, UserInformationDto>();
        }
    }
}
