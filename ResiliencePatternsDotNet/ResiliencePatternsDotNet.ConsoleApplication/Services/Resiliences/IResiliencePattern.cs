using ResiliencePatternsDotNet.ConsoleApplication.Services.RequestHandles;

namespace ResiliencePatternsDotNet.ConsoleApplication.Services.Resiliences
{
    public interface IResiliencePattern
    {
        void Execute(IRequestHandle requestHandle);
    }
}