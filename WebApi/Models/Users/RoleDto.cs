using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Users
{
    public class RoleDto : BaseUserDto
    {
        public Guid Id { get; set; }

        public IList<string> Roles { get; set; }
    }
}
