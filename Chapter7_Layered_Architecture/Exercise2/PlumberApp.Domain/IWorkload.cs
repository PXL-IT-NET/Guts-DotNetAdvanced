using System;
using System.Collections.Generic;

namespace PlumberApp.Domain
{
    public interface IWorkload
    {
        Guid Id { get; }
        string Name { get; }
        int Capacity { get; }
        IReadOnlyCollection<IJob> Jobs { get; }
        void AddJob(string description);
    }
}