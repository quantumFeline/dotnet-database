using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoResourcesWebApplication.Models
{
    public class User : IdentityUser
    {
        public Department department;
    }
}
