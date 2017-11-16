using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBook.ViewModel
{
    public class ChangePwdViewModel
    {  
        [DataType(DataType.Password), Required(ErrorMessage ="Old Password Required")]
        public string oldPassword { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage ="New Password Required")]
        public string newPassword { get; set; }

        [DataType(DataType.Password), Compare("newPassword")]
        public string confirmPassword { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, MaxLength(256)]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required, MaxLength(256)]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
