using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hair_Salon_Web_ASP.NET.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int post_id { get; set; }
        [Required]
        [StringLength(50)]
        public string? title { get; set; }
        //chua update lai database
        public DateTime? date { get; set; }
        public string? post_image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? description { get; set; }
    }
}
