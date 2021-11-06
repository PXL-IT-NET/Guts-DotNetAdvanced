using System.Collections.Generic;
using PlumberApp.Domain;

namespace PlumberApp.AppLogic
{
    public interface IWorkloadRepository
    {
        void Add(IWorkload workload);
        IReadOnlyList<IWorkload> GetAll();
        void SaveChanges(IWorkload workload);
    }
}
