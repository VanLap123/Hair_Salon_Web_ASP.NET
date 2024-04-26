using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.Repository;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hair_Salon_Web_ASP.NET.MyBackgroundService
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly RepositoryHairSalon _repo;

        public MyBackgroundService(RepositoryHairSalon repo)
        {
            _repo = repo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _repo.CanceledIfLate();

               
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
