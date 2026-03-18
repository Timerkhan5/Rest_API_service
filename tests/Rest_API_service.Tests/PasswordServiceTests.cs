using FluentAssertions;
using Rest_API_service.Services;

namespace Rest_API_service.Tests;

public class PasswordServiceTests
{
    private readonly PasswordService _service = new();

    [Fact]
    public void HashAndVerify_ShouldReturnTrue_ForSamePassword()
    {
        const string password = "StrongPassword123!";

        var hash = _service.Hash(password);
        var result = _service.Verify(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForDifferentPassword()
    {
        var hash = _service.Hash("password1");

        var result = _service.Verify("password2", hash);

        result.Should().BeFalse();
    }
}
