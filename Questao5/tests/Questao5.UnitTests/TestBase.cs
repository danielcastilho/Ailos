namespace Ailos.UnitTests;

public abstract class TestBase
{
    protected readonly ILogger<TestBase> Logger;

    protected TestBase()
    {
        Logger = Substitute.For<ILogger<TestBase>>();
    }

    protected static string GenerateGuid() => Guid.NewGuid().ToString();
}
