using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hair_Salon_Web_ASP.NET.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "Phone number must be 10 numbers")]
        public string phone_number { get; set; }
        [Required]
        public string password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? role { get; set; }
  
        public string? name { get; set; }
        public string? gender { get; set; }

        public string? address { get; set; }

        public string? about_user { get; set; }
        public virtual ICollection<Appointment>? appointment { get; set; }


    }
}
