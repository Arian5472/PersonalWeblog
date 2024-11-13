using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Weblog.Core.Domain.IdentityEntities;

namespace Weblog.Core.DTO
{
    public class UserRegisterDTO
    {
        [StringLength(30, ErrorMessage = "نام وارده بیش از حد طولانی است")]
        [Required(ErrorMessage = "نام خود را وارد نمایید")]
        public string? Name { get; set; }

        [StringLength(60, ErrorMessage = "ایمیل وارده بیش از حد طولانی است")]
        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "ایمیل خود را وارد نمایید")]
        public string? Email { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "گذرواژه باید بین 8 الی 20 کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "گذرواژه را وارد نمایید")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "تکرار گذواژه را وارد نمایید")]
        [Compare(nameof(Password), ErrorMessage = "گذرواژه با تکرار آن مطابقت ندارد")]
        public string? RepeatPassword { get; set; }

        [StringLength(360)]
        [Required(ErrorMessage = "بیو را وارد نمایید")]
        public string? Bio {  get; set; }

        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "تصویر پروفایل را بارگذاری نمایید.")]
        public IFormFile? Profile { get; set; }

        [Range(0, 2)]
        [Required(ErrorMessage = "نقش خود را انتخاب کنید")]
        public int? Level { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "کد دسترسی را وارد نمایید")]
        public string? LevelPass { get; set; }
    }

    public class UserSigninDTO
    {
        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "ایمیل خود را وارد نمایید")]
        public string? Email { set; get; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "گذرواژه باید بین 8 الی 20 کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "گذرواژه را وارد نمایید")]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class UserUpdateDTO
    {
        [StringLength(30, ErrorMessage = "نام وارده بیش از حد طولانی است")]
        [Required(ErrorMessage = "نام خود را وارد نمایید")]
        public string? Name { get; set; }

        [StringLength(60, ErrorMessage = "ایمیل وارده بیش از حد طولانی است")]
        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "ایمیل خود را وارد نمایید")]
        public string? Email { get; set; }

        [StringLength(360, ErrorMessage = "متن وارده بیش از حد طولانی است")]
        [Required(ErrorMessage = "بیو را وارد نمایید")]
        public string? Bio { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Profile { get; set; }
    }

    public class UserUpPassDTO
    {
        [StringLength(20, MinimumLength = 8, ErrorMessage = "گذرواژه باید بین 8 الی 20 کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "گذرواژه را وارد نمایید")]
        public string? CurrentPass { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "گذرواژه باید بین 8 الی 20 کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "گذرواژه را وارد نمایید")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "تکرار گذواژه را وارد نمایید")]
        [Compare(nameof(Password), ErrorMessage = "گذرواژه با تکرار آن مطابقت ندارد")]
        public string? RepeatPassword { get; set; }
    }

    public class UserEmailDTO
    {
        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "ایمیل خود را وارد نمایید")]
        public string? Email { set; get; }
    }

    public class UserResetPassDTO
    {
        [Required(ErrorMessage = "ایمیل موجود نیست")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "کد را وارد نمایید")]
        public string? Token { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "گذرواژه باید بین 8 الی 20 کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "گذرواژه را وارد نمایید")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "تکرار گذواژه را وارد نمایید")]
        [Compare(nameof(Password), ErrorMessage = "گذرواژه با تکرار آن مطابقت ندارد")]
        public string? RepeatPassword { get; set; }
    }

    public class UserResponseDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Bio { get; set; }
    }

    public static class UserExtension
    {
        public static UserResponseDTO ToUserResponse(this ApplicationUser user)
        {
            return new UserResponseDTO() { Id = user.Id, Name = user.Name, Email = user.Email, Bio = user.Bio };
        }
    }
}
