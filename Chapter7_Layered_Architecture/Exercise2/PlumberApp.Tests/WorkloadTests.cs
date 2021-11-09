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
    [ExerciseTestFixture("dotnet2", "H07", "Exercise02", @"PlumberApp.Domain\Workload.cs")]
    public class WorkloadTests : TestBase
    {
        private Type _workloadType;

        [SetUp]
        public void Setup()
        {
            _workloadType = typeof(Workload);
        }

        [MonitoredTest("Workload - Should implement IWorkload")]
        public void ShouldImplementIWorkload()
        {
            Assert.That(typeof(IWorkload).IsAssignableFrom(_workloadType), Is.True);
        }

        [MonitoredTest("IWorkload - Interface should not be changed")]
        public void ShouldNotHaveChangedIWorkload()
        {
            var filePath = @"PlumberApp.Domain\IWorkload.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("4F-31-32-04-D8-5C-87-75-18-7F-13-B0-51-61-41-58"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("Workload - Should have a private parameter-less constructor and private setters (for json conversion to work)")]
        public void ShouldHaveAPrivateParameterLessConstructorAndPrivateSettersForJsonConversionToWork()
        {
            var constructor = _workloadType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.IsPrivate);

            Assert.That(constructor, Is.Not.Null, "Cannot find a private constructor.");
            Assert.That(constructor.GetParameters().Length, Is.Zero, "The private constructor should not have parameters.");

            AssertHasPrivateSetter(_workloadType, nameof(IWorkload.Id));
            AssertHasPrivateSetter(_workloadType, nameof(IWorkload.Name));
            AssertHasPrivateSetter(_workloadType, nameof(IWorkload.Capacity));
        }

        [MonitoredTest("Workload - Should have a constructor that accepts a name and a capacity")]
        public void ShouldHaveAConstructorThatAcceptsANameAndACapacity()
        {
            string name = Guid.NewGuid().ToString();
            int capacity = Random.Next(1, int.MaxValue);
            CreateWorkload(name, capacity);
        }

        [MonitoredTest("Workload - Constructor - Should initialize properly")]
        public void Constructor_ShouldInitializeProperly()
        {
            string name = Guid.NewGuid().ToString();
            int capacity = Random.Next(1, int.MaxValue);

            IWorkload workload = CreateWorkload(name, capacity);

            Assert.That(workload.Name, Is.EqualTo(name), "The 'Name' is not initialized correctly.");
            Assert.That(workload.Capacity, Is.EqualTo(capacity), "The 'Capacity' is not initialized correctly.");
            Assert.That(workload.Id, Is.Not.EqualTo(Guid.Empty), "The constructor should generate and assign a Guid to the 'Id' property.");
            Assert.That(workload.Jobs, Is.Not.Null, "The 'Jobs' list should be an empty list.");
            Assert.That(workload.Jobs.Count, Is.Zero, "The 'Jobs' list should be an empty list.");
        }

        [MonitoredTest("Workload - Constructor - Empty name - Should throw an ArgumentException")]
        public void Constructor_EmptyName_ShouldThrowArgumentException()
        {
            string invalidName = null;
            int capacity = Random.Next(1, int.MaxValue);

            Assert.That(() => CreateWorkload(invalidName, capacity), Throws.ArgumentException,
                "An 'ArgumentException' should be thrown when the name is 'null'.");

            invalidName = "";
            Assert.That(() => CreateWorkload(invalidName, capacity), Throws.ArgumentException,
                "An 'ArgumentException' should be thrown when the name is an empty string.");
        }

        [MonitoredTest("Workload - Constructor - Zero or negative capacity - Should throw an ArgumentException")]
        public void Constructor_ZeroOrNegative_ShouldThrowArgumentException()
        {
            string name = Guid.NewGuid().ToString();
            int invalidCapacity = Random.Next(1, int.MaxValue) * -1;

            Assert.That(() => CreateWorkload(name, invalidCapacity), Throws.ArgumentException,
                "An 'ArgumentException' should be thrown when the capacity is negative.");

            invalidCapacity = 0;
            Assert.That(() => CreateWorkload(name, invalidCapacity), Throws.ArgumentException,
                "An 'ArgumentException' should be thrown when the capacity is zero.");
        }

        [MonitoredTest("Workload - ToString - Should return a string containing name and capacity")]
        public void ToString_ShouldReturnAStringContainingNameAndCapacity()
        {
            string name = Guid.NewGuid().ToString();
            int capacity = Random.Next(1, int.MaxValue);

            IWorkload workload = CreateWorkload(name, capacity);

            string workLoadAsText = workload.ToString();

            Assert.That(workLoadAsText, Does.StartWith(name), "The first part should be the name of the workload.");
            Assert.That(workLoadAsText, Does.Contain(capacity.ToString()), "The capacity is not found in the returned string.");
        }

        [MonitoredTest("Workload - AddJob - Should create an add the job")]
        public void AddJob_ShouldCreateAndAddTheJob()
        {
            //Arrange
            string name = Guid.NewGuid().ToString();
            int capacity = 10;
            IWorkload workload = CreateWorkload(name, capacity);

            string jobDescription = Guid.NewGuid().ToString();

            var originalJobCollection = workload.Jobs;

            //Act
            workload.AddJob(jobDescription);

            //Assert
            Assert.That(workload.Jobs.Count, Is.EqualTo(1), "The 'Jobs' property should contain 1 job.");
            IJob addedJob = workload.Jobs.First();
            Assert.That(addedJob, Is.Not.Null, "The added job should not be null.");
            Assert.That(addedJob.Description, Is.EqualTo(jobDescription), "The description of the added job is not correct.");
            Assert.That(addedJob.WorkloadId, Is.EqualTo(workload.Id), "The 'workloadId' of the added job is not correct.");

            Assert.That(workload.Jobs, Is.SameAs(originalJobCollection),
                "The collection that is returned by the 'Jobs' property should be the same object in memory than the collection that is returned by the 'Jobs' property after the construction of the workload. " +
                "Tip1: The 'Jobs' property should not have a setter. Use a backing field. " +
                "Tip2: List<IJob> implements the IReadOnlyCollection<IJob> interface.");
        }

        [MonitoredTest("Workload - AddJob - Maximum capacity reached - Should throw an InvalidOperationException")]
        public void AddJob_MaximumCapacityReached_ShouldThrowAnInvalidOperationException()
        {
            //Arrange
            string name = Guid.NewGuid().ToString();
            int capacity = 1;
            IWorkload workload = CreateWorkload(name, capacity);

            workload.AddJob(Guid.NewGuid().ToString());

            //Act + Assert
            Assert.That(() => workload.AddJob(Guid.NewGuid().ToString()), Throws.InvalidOperationException,
                "When a second job is added to a workload of capacity '1', an InvalidOperationException should be thrown.");
        }

        private IWorkload CreateWorkload(string name, int capacity)
        {
            ConstructorInfo constructor = _workloadType
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(c => c.IsAssembly || c.IsPublic);

            Assert.That(constructor, Is.Not.Null, "Cannot find a non-private constructor.");
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(2), "Cannot find a constructor that accepts 2 parameters");

            Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)), "The first parameter should be a string (name).");
            Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(int)), "The second parameter should be an integer (capacity).");

            try
            {
                return constructor.Invoke(new object[] { name, capacity }) as IWorkload;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }
}