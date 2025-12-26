using System;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class FakeSmsService : ISmsService
    {
        public Task SendAsync(string phone, string message)
        {
            Console.WriteLine("SMS To: " + phone);
            Console.WriteLine("Message: " + message);
            Console.WriteLine("----");
            return Task.CompletedTask;
        }
    }
}
