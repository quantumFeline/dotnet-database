﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoResourcesWebApplication
{
    public class RegisterViewModel
    {
        //public RegisterViewModel()
        //{
        //    // NOP
        //}

        [Required]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Department")]
        public Department department { get; set; }


        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [Display(Name = "Password confirmation")]
        [DataType(DataType.Password)]
        public string passwordConfirm { get; set; }
    }
}
