namespace Rest_API_service.Services;

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
