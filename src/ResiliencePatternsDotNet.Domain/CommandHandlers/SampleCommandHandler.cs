using System;
using ResiliencePatternsDotNet.Domain.Commands;

namespace ResiliencePatternsDotNet.Domain.CommandHandlers
{
    public class SampleCommandHandler : CommandHandler<SampleCommand, int>
    {
        public override int Handle(SampleCommand command) 
            => new Random().Next();
    }
}