using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.DataTransferObject
{
    public class UserInformationDto : UserInformation
    {
        public string StringId { get; set; }
        public string Email { get; set; }
    }
}
