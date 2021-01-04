
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Text;  
namespace Core.Domain
{
    [Table("AspNetUsers")]
    public class User : IdentityUser
    {
    }
}
