using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PlumberApp.Domain;

namespace PlumberApp.Infrastructure.Storage
{
    public class WorkloadFileRepository
    {
        private readonly string _workloadFileDirectory;

        public WorkloadFileRepository(string workloadFileDirectory)
        {
           
        }

        public void Add(IWorkload workload)
        {
            SaveWorkload(workload);
        }

        public IReadOnlyList<IWorkload> GetAll()
        {
            //TODO: read all workload files in the directory, convert them to IWorkload objects and return them
            //Tip: use helper methods that are given (ReadWorkloadFromFile)
            return null;
        }

        public void SaveChanges(IWorkload workload)
        {
            SaveWorkload(workload);
        }

        private IWorkload ReadWorkloadFromFile(string workLoadFilePath)
        {
            //TODO: read the json in a workload file and deserialize the json into an IWorkload object
            //Tip: use helper methods that are given (ConvertJsonToWorkload)
            return null;
        }

        private void SaveWorkload(IWorkload workload)
        {
            //TODO: save the workload in a json format in a file
            //Tip: use helper methods that are given (GetWorkloadFilePath, ConvertWorkloadToJson)
        }

        private string ConvertWorkloadToJson(IWorkload workload)
        {
            string json = JsonConvert.SerializeObject(workload, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return json;
        }

        private IWorkload ConvertJsonToWorkload(string json)
        {
            Workload workload =  JsonConvert.DeserializeObject<Workload>(json, new JsonSerializerSettings
            {
                ContractResolver = new JsonAllowPrivateSetterContractResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.Auto
            });
            return workload as IWorkload;
        }

        private string GetWorkloadFilePath(Guid workLoadId)
        {
            string fileName = $"Workload_{workLoadId}.json";
            return Path.Combine(_workloadFileDirectory, fileName);
        }
    }
}
