using System;
using System.Linq;
using System.Reflection;
using Guts.Client.Classic;
using Guts.Client.Shared;
using Guts.Client.Shared.TestTools;
using NUnit.Framework;
using PlumberApp.Domain;

namespace PlumberApp.Tests
{
    [ExerciseTestFixture("dotnet2", "H07", "Exercise02", @"PlumberApp.Domain\Job.cs")]
    public class JobTests : TestBase
    {
        private Type _jobType;

        [SetUp]
        public void Setup()
        {
            _jobType = typeof(Job);
        }

        [MonitoredTest("Job - Should implement IJob")]
        public void ShouldImplementIJob()
        {
            Assert.That(typeof(IJob).IsAssignableFrom(_jobType), Is.True);
        }

        [MonitoredTest("IJob - Interface should not be changed")]
        public void ShouldNotHaveChangedIJob()
        {
            var filePath = @"PlumberApp.Domain\IJob.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("33-60-30-41-D0-98-FA-46-FF-C1-F7-0D-97-2E-B9-FC"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Job - Should only be visible to the domain layer")]
        public void ShouldOnlyBeVisibleToTheDomainLayer()
        {
            Assert.That(_jobType.IsNotPublic,
                "Only IJob should be visible to the other layers. The Job class itself can be encapsulated in the domain layer.");
        }

        [MonitoredTest("Job - Should have a private parameter-less constructor and private setters (for json conversion to work)")]
        public void ShouldHaveAPrivateParameterLessConstructorAndPrivateSettersForJsonConversionToWork()
        {
            var constructor = _jobType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.IsPrivate);

            Assert.That(constructor, Is.Not.Null, "Cannot find a private constructor.");
            Assert.That(constructor.GetParameters().Length, Is.Zero, "The private constructor should not have parameters.");

            AssertHasPrivateSetter(_jobType, nameof(IJob.Id));
            AssertHasPrivateSetter(_jobType, nameof(IJob.Description));
            AssertHasPrivateSetter(_jobType, nameof(IJob.WorkloadId));
        }

        [MonitoredTest("Job - Should have a constructor that accepts a description and a workload id")]
        public void ShouldHaveAConstructorThatAcceptsADescriptionAndAWorkloadId()
        {
            string description = Guid.NewGuid().ToString();
            Guid workloadId = Guid.NewGuid();
            CreateJob(description, workloadId);
        }

        [MonitoredTest("Job - Constructor - Should initialize properly")]
        public void Constructor_ShouldInitializeProperly()
        {
            string description = Guid.NewGuid().ToString();
            Guid workloadId = Guid.NewGuid();

            IJob job = CreateJob(description, workloadId);

            Assert.That(job.Description, Is.EqualTo(description), "The 'Description' is not initialized correctly.");
            Assert.That(job.WorkloadId, Is.EqualTo(workloadId), "The 'WorkloadId' is not initialized correctly.");
            Assert.That(job.Id, Is.Not.EqualTo(Guid.Empty), "The constructor should generate and assign a Guid to the 'Id' property.");
        }

        [MonitoredTest("Job - ToString - Should return the description")]
        public void ToString_ShouldReturnDescription()
        {
            string description = Guid.NewGuid().ToString();
            Guid workloadId = Guid.NewGuid();

            IJob job = CreateJob(description, workloadId);

            Assert.That(job.ToString(), Is.EqualTo(description));
        }

        private IJob CreateJob(string description, Guid workLoadId)
        {
            ConstructorInfo constructor = _jobType
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(c => c.IsAssembly || c.IsPublic);

            Assert.That(constructor, Is.Not.Null, "Cannot find a non-private constructor.");
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(2), "Cannot find a constructor that accepts 2 parameters");

            Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)), "The first parameter should be a string (description).");
            Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(Guid)), "The second parameter should be a Guid (workloadId).");

            try
            {
                return constructor.Invoke(new object[] { description, workLoadId }) as IJob;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }
}