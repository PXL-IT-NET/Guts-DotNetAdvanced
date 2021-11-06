using System;
using System.Collections.Generic;
using PlumberApp.Domain;

namespace PlumberApp.Tests
{
    internal class TestWorkload : IWorkload
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public IReadOnlyCollection<IJob> Jobs { get; set; }
        public void AddJob(string description)
        {
            throw new NotImplementedException();
        }
    }
}