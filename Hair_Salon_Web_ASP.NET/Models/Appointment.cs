using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hair_Salon_Web_ASP.NET.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int app_id { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public string booking_time { get; set; }
        public string? finish_time { get; set; }
       
        public string? status { get; set; }
        public int emp_id_chosen { get; set; }
        [ForeignKey("emp_id_chosen")]
        public virtual User? user { get; set; }
        public int? user_id_book { get; set; }
        public int ser_id { get; set;}
        [ForeignKey("ser_id")]
        public virtual Service? service { get; set; }
    }
}
