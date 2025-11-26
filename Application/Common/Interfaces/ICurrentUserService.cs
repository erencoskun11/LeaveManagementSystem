namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        // Giriş yapmış kullanıcının ID'si (ClaimTypes.NameIdentifier)
        string? UserId { get; }

        // Giriş yapmış kullanıcının Email'i (ClaimTypes.Email)
        string? Email { get; }

        // Kullanıcı giriş yapmış mı?
        bool IsAuthenticated { get; }
    }
}