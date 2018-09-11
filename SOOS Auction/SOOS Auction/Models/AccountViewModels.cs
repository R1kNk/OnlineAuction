using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOOS_Auction.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [StringLength(20,ErrorMessage ="Длина имени не должна превышать 20 символов!")]
        [Display(Name = "Логин пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Поле электронной почты должно быть заполнено")]
        [StringLength(20, ErrorMessage = "Длина имени не должна превышать 20 символов!")]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Логин пользователя")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен иметь длину как минимум {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Поле подтверждения пароля не совпадает с паролем")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Пол")]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^(\+375|80)(29|25|44|33)(\d{3})(\d{2})(\d{2})$", ErrorMessage = "Номер телефона введен некорректно")]
        [Display(Name ="Номер телефона")]
        public string TelephoneNumber { get; set; }

        [Required]
        [Display(Name = "Место проживания")]
        public string UserLocation { get; set; }
    }

    public class UserViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "Длина имени не должна превышать 20 символов!")]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Логин пользователя")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Пол")]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^(\+375|80)(29|25|44|33)(\d{3})(\d{2})(\d{2})$", ErrorMessage = "Номер телефона введен некорректно")]
        [Display(Name = "Номер телефона")]
        public string TelephoneNumber { get; set; }

        [Required]
        [Display(Name = "Место проживания")]
        public string UserLocation { get; set; }

        public List<string> Roles { get; set; }
    }

    public class UserEditModel : UserViewModel
    {
        [Display(Name ="Роль: Админ")]
        public bool AdminRole { get; set; }

        [Display(Name = "Роль: Модератор")]
        public bool ModerRole { get; set; }

    }

    public class ChangeAvatarModel
    {
        public string AvatarUrl { get; set; }
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }
    public class AddReviewModel
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
