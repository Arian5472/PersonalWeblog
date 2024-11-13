using System.ComponentModel.DataAnnotations;

namespace Weblog.Core.Domain.Entities
{
    public class Subscriber
    {
        [Key]
        [StringLength(60, ErrorMessage = "ایمیل وارده بیش از حد طولانی است")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
        [Required(ErrorMessage = "ایمیل خود را وارد نمایید")]
        public string? Email { get; set; }

        public bool? Verified { get; set; } = false;

        [Range(10000, 99999)]
        public int? VerificationCode {  get; set; }
    }
}
