# Guts-DotNetAdvanced
In this repository you can find (visual studio) start solutions for the exercises of the **.NET Advanced** course of **PXL-IT**.

The exercises are grouped by chapter. There is a (visual studio) solution for each chapter.
A chapter solution contains multiple projects:
- A WPF project for each exercise. E.g. *Exercise01*
- A test project for each WPF project. E.g. *Exercise01.Tests*

![alt text][img_projects]

The WPF solutions are waiting for you to complete them.
The matching test projects contain **automated tests** that can be run to check if your solution is correct.

## Chapters
The assignments for each chapter can be found in de README.md in the folder of the chapter or by clicking on one of the links below:
* [Chapter 2 - WPF Controls](Chapter2_WPF_Controls/README.md)
* [Chapter 3 - WPF Layout](Chapter3_WPF_Layout/README.md)
* [Chapter 4 - WPF Databinding](Chapter4_WPF_Databinding/README.md)
* [Chapter 5 - Unit Testing](Chapter5_Unit_Testing/README.md)
* [Chapter 10 - ADO.NET Transactions](Chapter10_ADO_Transactions/README.md)
* [Chapter 11 - LINQ](Chapter11_Linq/README.md)
* [Chapter 12 - Entity Framework](Chapter12_Entity_Framework/README.md)

## Getting Started

First you need to have the files in this repository on your local machine. 
So you need to clone the files in the online repository on your local machine using git.
Click on "Clone or download" at the top right of this page.
Click on "Open in Desktop" if you use the *GitHub Desktop* tool or copy the web URL and clone using the command line or your favorite git tool (e.g. GitKraken).

![alt text][img_clone]

You now have local copy (a clone) of the online repository on your local machine. 

PS: In git the online (remote) repository is refered to as *origin*. 

PS2: **Do not fork the repository**. 
The forked repo will be publicly visible enabling other students to copy your work. 
**Forking a repo will be considered as commiting fraude**.

#### Register on [guts-web.pxl.be](https://guts-web.pxl.be)
To be able to send your tests results to the Guts servers you need to register via [guts-web.pxl.be](https://guts-web.pxl.be/register).
After registration you will have the credentials you need to succesfully run automated tests for an exercise.

#### Start working on an exercise
Let's assume you want to make exercise 5 of chapter 5.
1. Open the solution in the folder "Chapter 5". You can do this by doubleclicking on the **.sln** file or by opening visual studio, clicking on *File -> Open -> Project/Solution...* and selecting the **.sln** file.
![alt text][img_open_solution]
2. **Build the solution** (Menu: Build -> Build Solution or Ctrl+Shift+B)
3. Locate the project "Exercise 5" and set it as your startup project
![alt text][img_startup_project]
4. Write the code you need to write

#### Run the automated tests
Let's assume you are working on exercise 5 of chapter 5.
1. Open the *Test Explorer* window (Menu: Test -> Windows -> Test Explorer)
2. In the top left corner, right click on the down arrow of the *group by* button and group the automated tests by project. (If you don't see any tests appearing, you probably should (re)build your solution)
![alt text][img_group_tests]
3. Right click on the project that matches your exercise and click on *Run selected tests*
4. The first time you run a test a popup may appear thats asks you to log in. You should fill in your credentials from [guts-web.pxl.be](https://guts-web.pxl.be).
![alt text][img_login_vs]

**Attention**: your results will only be sent to the GUTS server when you run all the tests of a test fixture.

#### Inspect the test results
Tests that pass will be green. Tests that don't pass will be red. 

The *name of the test* gives an indication of what is tested in the automated test.
If you click on a test you can also read more detailed messages that may help you to find out what is going wrong.

![alt text][img_test_detail]

Although it is not a guarantee, having all tests green is a good indication that you completed the exercise correctly.

Once you finished an exercise it is recommended to **create a local commit** that stores your changes.

In case of disaster (e.g. a laptop crash) you can still recover the source code of your solutions by navigating to the details of an exercise via [guts-web.pxl.be](https://guts-web.pxl.be).

#### Check your results online
Test results of all students are sent to the Guts servers.
You can check your progress and compare with the averages of other students via [guts-web.pxl.be](https://guts-web.pxl.be).
Login, go to ".NET Advanced" in the navigation bar and select the chapter you want to view.
![alt text][img_chapter_contents]

## How do I deal with updates / bugfixes on the start code?
When you are working on the exercises of a chapter and you get notified that the start code changed (e.g. after a bug fix in one of the tests), you want to get the latest version of the tests **without losing any work** you already did.
There are 2 options:

#### Option1 (recommended): commit your changes
The best option is to really **embrace git and create local commits** that contain your changes (e.g. each time you finish an exercise). 
You can then safely do a **git pull origin master**. 
This wil create a (local) merge commit that contains an updated version of the tests (from the remote repository) merged with the changes you made. 

#### Option 2: stash your changes
If you don't have local commits that contain your changes, you can *stash* (or set aside) your local changes, then pull (or get) the latest version of the online repository to finally reapply the stashed local changes.
* Open a command prompt
* Navigate to the local folder in which you cloned the online repository (**!the root folder, not a folder for a specific chapter or exercise!**)
* Save your local changes by executing the command **git stash**
* Do a pull request that overwrites the local code with the code in the online repository: **git pull origin master**
* Reapply your stashed changes: **git stash apply**
* Clear the stash (in case you need to do this operation again in the future): **get stash clear**


[img_projects]:Images/projects.png "Solution for chapter five with its projects"
[img_download]:Images/download.png "Download repository"
[img_fork]:Images/fork.png "Fork repository"
[img_clone]:Images/clone.png "Clone repository"
[img_open_solution]:Images/open_solution.png "Open solution"
[img_startup_project]:Images/startup_project.png "Choose startup project"
[img_group_tests]:Images/group_tests.png "Group tests by project"
[img_test_detail]:Images/test_detail.png "Details of a test result"
[img_login_vs]:Images/login_vs.png "Visual studio login"
[img_chapter_contents]:Images/chaptercontents.png "Chapter contents"