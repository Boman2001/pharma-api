using System;
using Core.Domain.Models;

namespace WebApi.Models.AdditionalExaminationTypes
{
    public class UpdateAdditionalExaminationTypeDto : BaseAdditionalExaminationTypeDto
    {
        public int Id { get; set; }
    }
}