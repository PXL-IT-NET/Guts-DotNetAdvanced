using System;

namespace PlumberApp.Domain
{
    public interface IJob
    {
        Guid Id { get; }
        string Description { get; }
        Guid WorkloadId { get; }
    }
}