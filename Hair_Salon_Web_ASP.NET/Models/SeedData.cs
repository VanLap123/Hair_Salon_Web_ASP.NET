using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml.Linq;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Hair_Salon_Web_ASP.NET.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any movies.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }
                context.Users.AddRange(
                    new User
                    {
                        phone_number = "0901278074",
                        password = BCrypt.Net.BCrypt.HashPassword("1", 12),
                       
                        role = "Admin",
                        name = "Nguyen Van Lap",
                        gender = "Male",
                        address = "Tan Thanh, Binh Tan, Vinh Long provice",
                        about_user = "I am an administrator"
                    },
                    new User
                    {
                        phone_number = "0901278075",
                        password = BCrypt.Net.BCrypt.HashPassword("1", 12),
                       
                        role = "Employee",
                        name = "Ho Kien Vinh",
                        gender = "Male",
                        address = "An Nhon, Cai Tau, Soc Trang provice",
                        about_user = "I am an employee"
                    },
                     new User
                     {
                         phone_number = "0901278076",
                         password = BCrypt.Net.BCrypt.HashPassword("1", 12),
                         
                         role = "Employee",
                         name = "Nguyen Ngoc Duy Chuong",
                         gender = "Male",
                         address = "An Nhon, Cai Tau, Soc Trang provice",
                         about_user = "I am an employee"
                     },
                      new User
                      {
                          phone_number = "0901278077",
                          password = BCrypt.Net.BCrypt.HashPassword("1", 12),
                          role = "Employee",
                     
                          name = "Nguyen Nhat Khang",
                          gender = "Male",
                          address = "3/2 street, Can Tho city",
                          about_user = "I am an employee"
                      },
                    new User
                    {
                        phone_number = "0901278078",
                        password = BCrypt.Net.BCrypt.HashPassword("1", 12),
                        role = "Client",
                       
                        name = "Nguyen Phi Hung",
                        gender = "Male",
                        address = "Tan Thanh, Binh Tan, Vinh Long provice",
                        about_user = "I am a client"
                    }
                 ); ;
           
                context.SaveChanges();
            }
        }
    }
}
