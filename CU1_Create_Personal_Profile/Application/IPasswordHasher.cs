namespace CestaJusta.CU1.CreateProfile.Application;

public interface IPasswordHasher
{
    PasswordHashResult Hash(string password);
}

public sealed record PasswordHashResult(string Hash, string Salt);