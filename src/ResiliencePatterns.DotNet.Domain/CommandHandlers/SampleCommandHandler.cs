using System;
using ResiliencePatterns.DotNet.Domain.Commands;

namespace ResiliencePatterns.DotNet.Domain.CommandHandlers
{
    public class SampleCommandHandler : CommandHandler<SampleCommand, int>
    {
        public override int Handle(SampleCommand command) 
            => new Random().Next();
    }
}