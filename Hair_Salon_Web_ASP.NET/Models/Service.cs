using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hair_Salon_Web_ASP.NET.Models
{
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ser_id { get; set; }
        
        [Required(ErrorMessage = "Price is required.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a valid price.")]
        public string price { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a positive integer.")]
        public int time_to_cut { get; set; }
        public string? ser_image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? description { get; set; }
        public virtual ICollection<Appointment>? appointment { get; set; }

    }
}
