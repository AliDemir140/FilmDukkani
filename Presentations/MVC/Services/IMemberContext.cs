namespace MVC.Services
{
    public interface IMemberContext
    {
        bool IsAuthenticated { get; }
        string? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        string? Token { get; }
    }
}
