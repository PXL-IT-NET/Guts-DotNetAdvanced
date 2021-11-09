using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;
using PlumberApp.AppLogic;
using PlumberApp.Domain;
using PlumberApp.Infrastructure.Storage;
using PlumberApp.Tests.Builders;

namespace PlumberApp.Tests
{
    [ExerciseTestFixture("dotnet2", "H07", "Exercise02", @"PlumberApp.Infrastructure\Storage\WorkloadFileRepository.cs")]
    public class WorkloadFileRepositoryTests : TestBase
    {
        private WorkloadFileRepository _repository;
        private string _workloadDirectory;

        [SetUp]
        public void BeforeEachTest()
        {
            _workloadDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testworkloads");
            _repository = new WorkloadFileRepository(_workloadDirectory);
        }

        [TearDown]
        public void AfterEachTest()
        {
            if (Directory.Exists(_workloadDirectory))
            {
                Directory.Delete(_workloadDirectory, true);
            }
        }

        [MonitoredTest("IWorkloadRepository - Should not have changed interface")]
        public void ShouldNotHaveChangedIWorkloadRepository()
        {
            var filePath = @"PlumberApp.AppLogic\IWorkloadRepository.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("FD-74-10-52-63-16-02-39-00-79-94-DC-03-B0-C3-60"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("WorkloadFileRepository - Should implement IWorkloadRepository")]
        public void ShouldImplementIWorkloadRepository()
        {
            var type = typeof(WorkloadFileRepository);
            Assert.That(typeof(IWorkloadRepository).IsAssignableFrom(type), Is.True);
        }

        [MonitoredTest("WorkloadFileRepository - Should only be visible to the infrastructure layer")]
        public void ShouldOnlyBeVisibleToTheInfrastructureLayer()
        {
            var type = typeof(WorkloadFileRepository);
            Assert.That(type.IsNotPublic);
        }

        [MonitoredTest("WorkloadFileRepository - Constructor - Should create the file directory")]
        public void Constructor_ShouldCreateTheFileDirectory()
        {
           Assert.That(Directory.Exists(_workloadDirectory), Is.True);
        }

        [MonitoredTest("WorkloadFileRepository - Add - Should save a json version of the workload in a file")]
        public void Add_ShouldSaveAJsonVersionOfTheWorkloadInAFile()
        {
            //Arrange
            IWorkload workload = new WorkloadBuilder().Build();

            //Act
            _repository.Add(workload);

            //Assert
            AssertThatWorkloadFileExists(workload);
        }

        [MonitoredTest("WorkloadFileRepository - GetAll - Should retrieve all added workloads")]
        public void GetAll_ShouldRetrieveAllAddedWorkloads()
        {
            //Arrange
            IWorkload workload1 = new WorkloadBuilder().Build();
            IWorkload workload2 = new WorkloadBuilder().Build();

            try
            {
                _repository.Add(workload1);
                _repository.Add(workload2);
            }
            catch (Exception)
            {
                Assert.Fail(
                    $"Make sure that the test '{nameof(Add_ShouldSaveAJsonVersionOfTheWorkloadInAFile)}' is green, before attempting to make this test green.");
            }
            
            //Act
            IReadOnlyList<IWorkload> allWorkloads = _repository.GetAll();

            //Assert
            Assert.That(allWorkloads.Count, Is.EqualTo(2), "2 workloads should be returned after adding 2 workloads.");

            var workloadMatch1 = allWorkloads.FirstOrDefault(wl => wl.Id == workload1.Id);
            AssertWorkloadEquality(workloadMatch1, workload1, "The first added workload is not the same than the matching retrieved workload.");

            var workloadMatch2 = allWorkloads.FirstOrDefault(wl => wl.Id == workload2.Id);
            AssertWorkloadEquality(workloadMatch2, workload2, "The second added workload is not the same than the matching retrieved workload.");
        }

        [MonitoredTest("WorkloadFileRepository - SaveChanges - Should overwrite the matching workload file")]
        public void SaveChanges_ShouldOverwriteTheMatchingWorkloadFile()
        {
            //Arrange
            TestWorkload workload = new WorkloadBuilder().Build() as TestWorkload;

            try
            {
                _repository.Add(workload);
            }
            catch (Exception)
            {
                Assert.Fail(
                    $"Make sure that the test '{nameof(Add_ShouldSaveAJsonVersionOfTheWorkloadInAFile)}' is green, before attempting to make this test green.");
            }

            string newName = "EditedName";
            int newCapacity = workload.Capacity + 1;

            //Act
            workload.Name = newName;
            workload.Capacity = newCapacity;
            _repository.SaveChanges(workload);

            //Assert
            IReadOnlyList<IWorkload> allWorkloads = null;
            try
            {
                allWorkloads = _repository.GetAll();
            }
            catch (Exception)
            {
                Assert.Fail(
                    $"Make sure that the test '{nameof(GetAll_ShouldRetrieveAllAddedWorkloads)}' is green, before attempting to make this test green.");
            }

            Assert.That(allWorkloads.Count, Is.EqualTo(1), "Only one workload should be returned by 'GetAll' after adding 1 workload and updating it.");

            IWorkload updatedWorkload = allWorkloads.First();

            Assert.That(updatedWorkload.Name, Is.EqualTo(newName), "A change in the name is not saved correctly.");
            Assert.That(updatedWorkload.Capacity, Is.EqualTo(newCapacity), "A change in the capacity is not saved correctly.");
        }

        private void AssertThatWorkloadFileExists(IWorkload workload)
        {
            string expectedFilePath = Path.Combine(_workloadDirectory, $"Workload_{workload.Id}.json");
            Assert.That(File.Exists(expectedFilePath), Is.True,
                $"After adding a workload with id {workload.Id}, a file '{expectedFilePath}' should exist.");
        }

        private void AssertWorkloadEquality(IWorkload workload1, IWorkload workload2, string errorMessage)
        {
            Assert.That(workload1.Id, Is.EqualTo(workload2.Id), $"{errorMessage} - The id's don't match.");
            Assert.That(workload1.Capacity, Is.EqualTo(workload2.Capacity), $"{errorMessage} - The capacities don't match.");
            Assert.That(workload1.Name, Is.EqualTo(workload2.Name), $"{errorMessage} - The names don't match.");
            Assert.That(workload1.Jobs, Is.Not.Null, $"{errorMessage} - The jobs of one of the workload is null.");
            Assert.That(workload2.Jobs, Is.Not.Null, $"{errorMessage} - The jobs of one of the workload is null.");
            Assert.That(workload1.Jobs.Count, Is.EqualTo(workload2.Jobs.Count), $"{errorMessage} - The jobs don't match.");
        }

    }
}