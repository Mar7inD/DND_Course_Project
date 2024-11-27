public class TokenService
{
    private string? _token;

    public string? GetToken()
    {
        return _token;
    }

    public void SetToken(string token)
    {
        _token = token;
    }

    public void ClearToken()
    {
        _token = null;
    }
}