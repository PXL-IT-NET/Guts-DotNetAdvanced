using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using Guts.Client.Classic;
using Guts.Client.Classic.TestTools.WPF;
using Guts.Client.Shared;
using Moq;
using NUnit.Framework;
using PlumberApp.AppLogic;
using PlumberApp.Domain;
using PlumberApp.Tests.Builders;
using PlumberApp.UI;

namespace PlumberApp.Tests
{
    [ExerciseTestFixture("dotnet2", "H07", "Exercise02", @"PlumberApp.UI\MainWindow.xaml.cs")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests : TestBase
    {
        private Mock<IWorkloadRepository> _workLoadRepositoryMock;
        private MainWindow _window;
        private List<IWorkload> _allWorkloads;

        [SetUp]
        public void BeforeEachTest()
        {
            _allWorkloads = new List<IWorkload>();
            for (int i = 0; i < Random.Next(2, 11); i++)
            {
                _allWorkloads.Add(new WorkloadBuilder().Build());
            }
            _workLoadRepositoryMock = new Mock<IWorkloadRepository>();
            _workLoadRepositoryMock.Setup(repo => repo.GetAll()).Returns(_allWorkloads);
            _window = new MainWindow(_workLoadRepositoryMock.Object);
            _window.Show();
        }

        [TearDown]
        public void AfterEachTest()
        {
            _window.Close();
        }

        [MonitoredTest("MainWindow - Constructor - Should retrieve all workloads and setup databinding")]
        public void Constructor_ShouldRetrieveAllWorkloadsAndSetupDatabinding()
        {
            _workLoadRepositoryMock.Verify(service => service.GetAll(), Times.Once,
                "The 'GetAll' method of the workload repository should be used.");

            Assert.That(_window.AllWorkloads, Is.EquivalentTo(_allWorkloads),
                "The workload instances returned by the repository should end up in the 'AllWorkloads' property.");

            Assert.That(_window.DataContext, Is.SameAs(_window),
                "The window should be its own DataContext.");
        }

        [MonitoredTest("MainWindow - SelectedWorkload - Should notify changes")]
        public void SelectedWorkload_ShouldNotifyChanges()
        {
            //Arrange
            IWorkload workload = new WorkloadBuilder().Build();
            bool selectedWorkloadChangeNotified = false;
            bool showSelectedWorkloadChangeNotified = false;
            _window.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainWindow.SelectedWorkload))
                {
                    selectedWorkloadChangeNotified = true;
                }

                if (args.PropertyName == nameof(MainWindow.ShowSelectedWorkload))
                {
                    showSelectedWorkloadChangeNotified = true;
                }
            };

            //Act
            _window.SelectedWorkload = workload;

            //Assert
            Assert.That(_window.SelectedWorkload, Is.SameAs(workload),
                "The 'SelectedWorkload' property is not set correctly.");

            Assert.That(selectedWorkloadChangeNotified, Is.True,
                "The 'PropertyChanged' event should be triggered for the property 'SelectedWorkload'.");

            Assert.That(showSelectedWorkloadChangeNotified, Is.True,
                "The 'PropertyChanged' event should also be triggered for the property 'ShowSelectedWorkload' " +
                "(This property might return a different value when the selected workload has changed).");
        }

        [MonitoredTest("MainWindow - ComboBox selection - Should set SelectedWorkload")]
        public void ComboBoxSelection_ShouldSetSelectedWorkload()
        {
            ComboBox comboBox = _window.FindVisualChildren<ComboBox>().FirstOrDefault();
            Assert.That(comboBox, Is.Not.Null, "Could not find a ComboBox in the window.");
            Assert.That(comboBox.Items.Count, Is.GreaterThan(0),
                "The ComboBox should contain some items. " +
                $"Maybe you need to make the test '{nameof(Constructor_ShouldRetrieveAllWorkloadsAndSetupDatabinding)}' green first.");

            comboBox.SelectedIndex = (comboBox.SelectedIndex + 1) % _allWorkloads.Count;

            //Assert
            Assert.That(_window.SelectedWorkload, Is.SameAs(comboBox.SelectedValue),
                "The 'SelectedWorkload' is not the same instance as the selected workload in the ComboBox.");
        }

        [MonitoredTest("MainWindow - Add workload button click - Should use the repository and update the UI")]
        public void AddWorkloadButtonClick_ShouldUseTheRepositoryAndUpdateTheUI()
        {
            Button addWorkloadButton = _window.FindVisualChildren<Button>().FirstOrDefault(b => (b.Content as string) == "Add workload");
            Assert.That(addWorkloadButton, Is.Not.Null, "Could not find a Button with content 'Add workload'.");

            TextBox workloadNameTextBox = _window.FindVisualChildren<TextBox>().FirstOrDefault(tb => tb.Name == "WorkloadNameTextBox");
            Assert.That(workloadNameTextBox, Is.Not.Null, "Could not find a TextBox with the name 'WorkloadNameTextBox'.");

            string workloadName = Guid.NewGuid().ToString();
            workloadNameTextBox.Text = workloadName;

            IWorkload addedWorkload = null;
            _workLoadRepositoryMock.Setup(repo => repo.Add(It.IsAny<IWorkload>()))
                .Callback((IWorkload workload) =>
                {
                    addedWorkload = workload;
                });

            addWorkloadButton.FireClickEvent();

            Assert.That(addedWorkload, Is.Not.Null, "The 'Add' method of the repository should be called.");
            Assert.That(addedWorkload.Name, Is.EqualTo(workloadName), "The workload that is passed in should contain the name filled in in the TextBox.");
            Assert.That(addedWorkload.Capacity, Is.EqualTo(10), "The workload that is passed in should have a capacity of 10.");

            Assert.That(_window.AllWorkloads, Contains.Item(addedWorkload),
                "The workload that is added in the repository should also be added to the 'AllWorkloads' collection.");

            Assert.That(_window.SelectedWorkload, Is.SameAs(addedWorkload),
                "The workload that is added in the repository should  also be the 'SelectedWorkload'.");

            Assert.That(workloadNameTextBox.Text, Is.Empty, "After adding the workload, the TextBox should be cleared.");
        }

        [MonitoredTest("MainWindow - Add job button click - No workload selected - Should do nothing")]
        public void AddJobButtonClick_NoWorkloadSelected_ShouldDoNothing()
        {
            Button addJobButton = _window.FindVisualChildren<Button>().FirstOrDefault(b => (b.Content as string) == "Add job");
            Assert.That(addJobButton, Is.Not.Null, "Could not find a Button with content 'Add job'.");

            TextBox jobDescriptionTextBox = _window.FindVisualChildren<TextBox>().FirstOrDefault(tb => tb.Name == "JobDescriptionTextBox");
            Assert.That(jobDescriptionTextBox, Is.Not.Null, "Could not find a TextBox with the name 'JobDescriptionTextBox'.");

            _window.SelectedWorkload = null;

            string jobDescription = Guid.NewGuid().ToString();
            jobDescriptionTextBox.Text = jobDescription;

            addJobButton.FireClickEvent();

            _workLoadRepositoryMock.Verify(repo => repo.SaveChanges(It.IsAny<IWorkload>()), Times.Never,
                "The 'SaveChanges' method of the repository should not have been called.");

            Assert.That(jobDescriptionTextBox.Text, Is.EqualTo(jobDescription), "The 'Text' of the job description TextBox should not change.");
        }

        [MonitoredTest("MainWindow - Add job button click - Workload selected - Should add job and update the UI")]
        public void AddJobButtonClick_WorkloadSelected_ShouldAddJobAndUpdateTheUI()
        {
            Button addJobButton = _window.FindVisualChildren<Button>().FirstOrDefault(b => (b.Content as string) == "Add job");
            Assert.That(addJobButton, Is.Not.Null, "Could not find a Button with content 'Add job'.");

            TextBox jobDescriptionTextBox = _window.FindVisualChildren<TextBox>().FirstOrDefault(tb => tb.Name == "JobDescriptionTextBox");
            Assert.That(jobDescriptionTextBox, Is.Not.Null, "Could not find a TextBox with the name 'JobDescriptionTextBox'.");

            string jobDescription = Guid.NewGuid().ToString();
            jobDescriptionTextBox.Text = jobDescription;

            var selectedWorkloadMock = new Mock<IWorkload>();
            IWorkload selectedWorkload = selectedWorkloadMock.Object;
            _window.SelectedWorkload = selectedWorkload;

            addJobButton.FireClickEvent();

            selectedWorkloadMock.Verify(wl => wl.AddJob(jobDescription), Times.Once,
                "The 'AddJob' method should be called on the selected workload. " +
                "The description should be the contents of the job description TextBox.");

            _workLoadRepositoryMock.Verify(repo => repo.SaveChanges(selectedWorkload), Times.Once,
                "After adding the job the workload should be saved in the repository.");

            Assert.That(jobDescriptionTextBox.Text, Is.Empty, "After adding the job, the TextBox should be cleared.");
        }
    }
}