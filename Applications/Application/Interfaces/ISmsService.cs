using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISmsService
    {
        Task SendAsync(string phone, string message);
    }
}
