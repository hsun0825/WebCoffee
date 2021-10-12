using Microsoft.Ajax.Utilities;

using System.ComponentModel.DataAnnotations;


namespace WebCoffee.Models
{
    public class MemberRegisterView
    {
        public Members newMember { get; set; }

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }

        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼輸入不一致")]
        [Required(ErrorMessage = "請輸入確認密碼")]
        public string PasswordCheck { get; set; }

    }
}