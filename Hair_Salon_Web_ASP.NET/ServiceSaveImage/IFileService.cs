using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hair_Salon_Web_ASP.NET.ServiceSaveImage
{
    public interface IFileService
    {
        Tuple<int, string> SaveImage(IFormFile imageFile);
        bool DeleteImage(string imageFileName);
    }
}