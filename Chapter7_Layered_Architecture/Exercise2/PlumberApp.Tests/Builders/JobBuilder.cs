using System;
using PlumberApp.Domain;

namespace PlumberApp.Tests.Builders
{
    internal class JobBuilder
    {
        private readonly TestJob _job;

        public JobBuilder()
        {
            _job = new TestJob
            {
                Id = Guid.NewGuid(),
                Description = Guid.NewGuid().ToString()
            };
        }

        public JobBuilder WithWorkloadId(Guid workloadId)
        {
            _job.WorkloadId = workloadId;
            return this;
        }

        public IJob Build()
        {
            return _job;
        }
    }
}