# Exercises - Chapter 12 - Entity Framework

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

#### Setup the database context (Lottery.Infrastructure)
The application will use Entity Framework (EF) to create, manage and access the database. 

The domain classes (the classes that represent entities in our lottery domain) can be found in a seperate project *Lottery.Domain*. 
In our domain we have 3 classes:
* LotteryGame: represents a kind of game (e.g. national lottery, keno, ...).
* Draw: happens when a the winning numbers of a lottery game are drawn (generated). The amount of numbers and the maximum number in a draw are determined by some lottery game properties (the minimum number is always 1).
* DrawNumber: holds information about a specific number that was drawn (the number itself and optionally a position for games in which the position of a number within the winning numbers, matters).

**Attention:** in this exercise the domain classes, foreign key properties and navigation properties are given. This will not always be the case (e.g. on you exam).

Now turn your attention to the *LotteryContext* class in the infrastructure layer (*Lottery.Infrastructure*). 

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

#### LotteryGameRepository.cs (Lottery.Infrastructure)
Implement the *GetAll* method. Use the automatic tests for guidance.

#### DrawRepository.cs (Lottery.Infrastructure)
Implement the *Find* method. Use the automatic tests for guidance.

Implement the *Add* method. Use the automatic tests for guidance.

#### DrawService.cs (Lottery.AppLogic)
The lottery application also has an application logic layer.

The *DrawService* is responsible for correctly generating a new draw for a certain lottery game 
and saving it in storage.
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

## Exercise 2

### General
In this exercise you'll be creating a banking application.

The *CustomersWindow* (the starting window) shows an overview of the customers of a bank:

![alt text][img_customers_window]
 
In the *New Customer* section a new customer can be added. To add a new customer the text boxes should be filled in and a city must be selected. 
When the *Add Customer* button is clicked a new customer is saved in the database and shown in the overview of customers. 
When not all fields are filled in correctly an error message is shown at the bottom.

Next to each customer in the overview, a button *Show accounts* is shown. When this button is clicked the accounts of that customer are shown in a dialog window (*AccountsWindow*). 

![alt text][img_accounts_window]

This window shows the accounts of the customer that was selected in the *CustomerWindow*. 
For each account the account number, the type and the current balance is shown.

Accounts can be added in a similair way as in the *CustomerWindow*.

Next to each account in the overview, a button *Transfer* is shown. When this button is clicked the accounts an internal transfer between the accounts of the customer can be made in a dialog window (*TransferWindow*).

![alt text][img_transfer_window]

Here you can select the target account (an account of the same customer) set an amount and transfer the funds by clicking on the button.
The transfer windows closes automatically and the balances of the accounts are automatically updated in the accounts window. 

### Implementation details

#### Complete the domain entities (Bank.Domain)
The domain classes (the classes that represent entities in our banking domain) can be found in the *Domain* layer. 
In our domain we have 3 classes:
* City: place with a name and a zipcode. The zipcode identifies each city uniquely.
* Customer: represents a customer of the bank. A customer is linked to a city via the *ZipCode* property. One customer can have many accounts.
* Account: banking account of a customer. There are different types of accounts. The type of the account is set by a property of the *AccountType* enum.

Tip: use the names and feedback of the automatic tests as guidance.

#### Setup the database context (Bank.Infrastructure)
The application will use Entity Framework (EF) to create, manage and access the database. 

**Attention:** the navigation properties (relations) are missing in the domain classes. It is up to you to define the correct relations.

Now turn your attention to the *BankContext* class. 

Add 3 *DbSet*s to the context one for *City*, one for *Customer* and one for *Account*. 
Now EF will know that these domain classes are a part of the database model. 
And thanks to the navigation properties EF will also infer that one customer can have many accounts.

You will still need to help EF a little bit to deduce a correct model from the domain classes. 
Use Fluent API to:
* Set *ZipCode* to be the primary key of a *City*.
* Set the *AcountNumber* to be the primary key of an *Account*.
* Make the *Name* and *FirstName* properties of a *Customer* required.
* Set the relation between a *Customer* and a *City*. EF will need your help to determine the correct foreign key column (*ZipCode*).

When the database is created it should be seeded with the 5 capital cities of Flanders:
* Antwerpen, zipcode = 2000
* Leuven, zipcode = 3000
* Hasselt, zipcode = 3500
* Brugge, zipcode = 8000
* Gent, zipcode = 9000

Now you should configure the context so that EF will work with SQL Server and use the connection string in the app.config of the UI project (*Bank.UI*).
Do this in the *OnConfiguring* method inside the if-test. 
**Do not change the constructors or place code outside the if-test in the *OnConfiguring* method. 
Otherwise the automatic tests will not function properly.**

The *BankContext* should be instantiated at the startup of the application inside *App.xaml.cs* (wiring). 
At that moment the *CreateOrUpdateDatabase* method of the context should be called. You need to make sure this method works. 

Tip: use the names and feedback of the automatic tests as guidance.

#### CityRepository.cs (Bank.Infrastructure)
The *CityRepository* class implements *ICityRepository* that defines 1 method:
* GetAllOrderedByZipCode: returns all cities in the database ordered by zip code

Use the automatic tests for guidance.

#### CustomerRepository.cs (Bank.Infrastructure)
The *CustomerRepository* class implements *ICustomerRepository* that defines 2 methods:
* GetAllWithAccounts: returns all customers with there accounts loaded
* Add: adds a new customer to the databse

Use the automatic tests for guidance.

#### AccountRepository.cs (Bank.Infrastructure)
The *AccountRepository* class implements *IAccountRepository* that defines 3 methods:
* GetByAccountNumber: retrieves a specific account from the database.
* Add: adds a new account to the database.
* CommitChanges: this should save the changes made to accounts retrieved from the database (e.g. update balances) after a transfer.

Use the automatic tests for guidance.

#### AccountService (Bank.AppLogic)
Implement the *AccountService* class. This service can be used to add a new account to a customer and to transfer many between the accounts of a customer.

Use the automatic tests for guidance.

#### CustomersWindow.xaml (.cs) (Bank.UI)
The XAML code is given. The C# code in the codebehind still needs some work.

Use the names and feedback of the automatic tests as guidance.

Note that the repositories and services (interfaces) are injected via the constructor.
There is also an *IWindowDialogService* that is injected. Use this object to open the *AccountsWindow* if needed. 
This eliminates the need to new up an instance of *AccountsWindow* directly and makes the class more testable. 

Don't forget to create and show an instance of the *CustomersWindow* during application startup (wiring).

#### AccountsWindow.xaml (.cs) (Bank.UI)
The accounts window is very similar to the customers window. 

Use the names and feedback of the automatic tests as guidance.

#### TransferWindow.xaml (.cs) (Bank.UI) 
Provide an implementation for the button click event handler. 

The unit tests for this class will guide you in the right direction.

[img_customers_window]:images/customers_window.png "Overview of customers"
[img_accounts_window]:images/accounts_window.png "Overview of the accounts of a customer"
[img_transfer_window]:images/transfer_window.png "Transfer an amount"																				