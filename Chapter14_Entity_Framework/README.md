# Exercises - Chapter 14 - Entity Framework

## Exercise 1

### General
In this exercise you'll be creating a lottery application.

The *LotteryWindow* (the starting window) lets you select a lottery game (e.g. Keno) and optionaly specify a date range. 

![alt text][img_lottery_window]
 
When the *Show draws* button is clicked all the draws of the selected lottery game within the daterange are shown:

![alt text][img_lottery_window_aftersearch]

After clicking on the *Show draws* button the *Add new draw* button becomes visible at the bottom of the screen. 
When this button is clicked, the application generates a new draw for the selected lottery game.

### Implementation details

#### Setup the database context (Lottery.Data)
The application will use Entity Framework (EF) to create, manage and access the database. 

The domain classes (the classes that represent entities in our lottery domain) can be found in a seperate project *Lottery.Domain*. 
In our domain we have 3 classes:
* LotteryGame: represents a kind of game (e.g. national lottery, keno, ...).
* Draw: happens when a the winning numbers of a lottery game are drawn (generated). The amount of numbers and the maximum number in a draw are determined by some lottery game properties (the minimum number is always 1).
* DrawNumber: holds information about a specific number that was drawn (the number itself and optionally a position for games in which the position of a number within the winning numbers, matters).

**Attention:** in this exercise the domain classes, foreign key properties and navigation properties are given. This will not always be the case (e.g. on you exam).

Now turn your attention to the *LotteryContext* class in the data layer (*Lottery.Data*). 

Add 2 *DbSet*s to the context one for *LotteryGame* and one for *Draw*. Now EF will know that *LotterGame* and *Draw* are a part of the database model. 
And thanks to the navigation properties EF will also infer that *DrawNumber* is a part of the model.

You will still need to help EF a little bit to deduce a correct model from the domain classes. 
Use Fluent API to:
* Make the *Name* property of a *LotteryGame* required.
* Define a composite primary key for *DrawNumber*. The combination of *DrawId* and *Number* should be the primary key.

When the database is created it should be seeded with (at least) 2 lottery games:
* The "National lottery" that has 6 numbers in a draw. The biggest number is 45.
* The "Keno" game that has 20 numbers in a draw. The biggest number is 70.

Tip: when seeding entities you also need to specify a primary key (even if it is an identity column in the database).

Now you should configure the context so that EF will work with SQL Server and use the connection string in the app.config of the UI project (*Lottery.UI*).
Do this in the *OnConfiguring* method inside the if-test. 
**Do not change the constructors or place code outside the if-test in the *OnConfiguring* method. 
Otherwise the automatic tests will not function properly.**

The *LotteryContext* will be instantiated at the startup of the application inside *App.xaml.cs*. 
At that moment the *CreateOrUpdateDatabase* method of the context will be called. You need to make sure this method works. 
**Do not change any code inside App.xaml.cs.** You only need to add an implementation to the *CreateOrUpdateDatabase* method.

#### LotteryGameRepository.cs (Lottery.Data)
Implement the *GetAll* method. Use the automatic tests for guidance.

#### DrawRepository.cs (Lottery.Data)
Implement the *Find* method. Use the automatic tests for guidance.

Implement the *Add* method. Use the automatic tests for guidance.

#### DrawService.cs (Lottery.Business)
The lottery application also has a business layer. The business layer uses the data layer to retrieve data 
and it is used by the presentation (UI) layer.

The *DrawService* is responsible for correctly generating a new draw for a certain lottery game 
and saving it in the database (using the data layer).
When the *DrawService* creates a draw it should take the following rules into account:
* The *LotteryGameId* of the draw must be set.
* The *Date* of the draw must be the current date.
* A correct amount of draw numbers should be generated.
* The positions of the draw numbers should start at 1 and increment by 1 for each next draw number.
* All numbers must be greather than or equal to 1.
* All numbers must be smaller than or equel to the maximum number of the lottery game.
* All numbers in a draw must be different.

#### LotteryWindow.xaml (.cs) (Lottery.UI)
The XAML code of the window already provived except for one thing. 
You still need to provide an *ItemTemplate* for the *ListView* that shows the draws of a game.

Each item (= a draw) should be displayed in a horizontal *StackPanel*. 
The *StackPanel* holds 2 instances of *TextBlock*: 
* One that displays the date of the draw (format = dd/MM/yyyy HH:mm).
* One that displays a comma seperated list of the numbers in the draw.

To display the list of numbers you will need to use the *DrawNumbersConverter* in the *Converters* folder of the UI project. 
The converter converts a collection of *DrawNumber*s to a string containing the numbers, seperated by commas an in the correct order (position).

Now use the automated tests on the *LotterWindow* class to implement the code behind of the UI. 

[img_lottery_window]:images/LotteryWindow.png "Lottery start window"
[img_lottery_window_aftersearch]:images/LotterWindow_AfterSearch.png "Draws of a lottery game"