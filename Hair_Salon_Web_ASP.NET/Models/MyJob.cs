using Hair_Salon_Web_ASP.NET.Repository;

namespace Hair_Salon_Web_ASP.NET.Models
{
    public class MyJob
    {
        
        private readonly RepositoryHairSalon _repo;

        public MyJob(ApplicationDbContext db)
        {
          
            _repo = new RepositoryHairSalon(db);
        }

        public void Execute()
        {
            
                _repo.CanceledIfLate();
            
        }
    }
}
