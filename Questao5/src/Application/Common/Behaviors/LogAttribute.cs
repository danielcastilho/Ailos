// Application/Common/Behaviors/LogAttribute.cs
namespace Ailos.Application.Common.Behaviors;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class LogAttribute : Attribute
{
    public LogLevel Level { get; }
    public bool IncludeRequest { get; }
    public bool IncludeResponse { get; }

    public LogAttribute(
        LogLevel level = LogLevel.Information,
        bool includeRequest = true,
        bool includeResponse = true
    )
    {
        Level = level;
        IncludeRequest = includeRequest;
        IncludeResponse = includeResponse;
    }
}
