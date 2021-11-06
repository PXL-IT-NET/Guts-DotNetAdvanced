using System;
using System.Collections.Generic;
using PlumberApp.Domain;

namespace PlumberApp.Tests.Builders
{
    internal class WorkloadBuilder
    {
        protected static Random Random = new Random();
        private readonly TestWorkload _workload;

        public WorkloadBuilder()
        {
            var jobs = new List<IJob>();
            _workload = new TestWorkload
            {
                Id = Guid.NewGuid(),
                Capacity = Random.Next(1, 101),
                Jobs = jobs,
                Name = Guid.NewGuid().ToString()
            };

            for (int i = 0; i < Random.Next(1,5); i++)
            {
                jobs.Add(new JobBuilder().WithWorkloadId(_workload.Id).Build());
            }
        }

        public IWorkload Build()
        {
            return _workload;
        }
    }
}