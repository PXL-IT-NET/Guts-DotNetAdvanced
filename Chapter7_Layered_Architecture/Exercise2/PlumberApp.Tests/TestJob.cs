using System;
using PlumberApp.Domain;

namespace PlumberApp.Tests
{
    internal class TestJob : IJob
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid WorkloadId { get; set; }
    }
}