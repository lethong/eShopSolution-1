using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class RegisterRequest
    {
        [Display(Name = "Tên", Prompt = "Tên")]
        public string FirstName { get; set; }

        [Display(Name = "Họ", Prompt = "Họ")]
        public string LastName { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [Display(Prompt = "Email")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại", Prompt = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tài khoản", Prompt = "Tài khoản")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu", Prompt = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Xác nhận mật khẩu", Prompt = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}