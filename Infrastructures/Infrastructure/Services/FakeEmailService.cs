using System;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class FakeEmailService : IEmailService
    {
        public Task SendAsync(string to, string subject, string body)
        {
            Console.WriteLine("Email To: " + to);
            Console.WriteLine("Subject: " + subject);
            Console.WriteLine("Body:");
            Console.WriteLine(body);
            Console.WriteLine("----");
            return Task.CompletedTask;
        }
    }
}
