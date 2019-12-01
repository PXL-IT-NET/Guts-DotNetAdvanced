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

## Exercise 2

### General
In this exercise you'll be creating a banking application.

The *CustomersWindow* (the starting window) shows an overview of the customers of a bank:

![alt text][img_customers_window]
 
When the *Add Customer* button is clicked a new (empty) row/customer is added to the table. 
When the selected row is a new customer and the *Save* button is clicked, the new customer is added in the database.

![alt text][img_customers_window_addrow]

The city of a customer can be set by selecting the city in a dropdown list.

![alt text][img_customers_window_selectcity]

By double clicking on a cell in the table you can start editing that cell:

![alt text][img_customers_window_editrow]

When the *Save* button is clicked the changes of the selected row/customer are persisted in the database.

When the *Show Accounts* button is clicked, a new window (*AccountsWindow*) is opened as a dialog. 


![alt text][img_accounts_window]

This window shows the accounts of the customer that was selected in the *CustomerWindow*. 
Accounts can be edited and added in a similair way as in the *CustomerWindow*. 
The *balance* colunm, however, is read-only. The balance of an account can only be changed by selecting an account and clicking on the *Transfer Amount* button. 
This button opens a new window (*TransferWindow*) as a dialog:

![alt text][img_transfer_window]

Here you can select the target account (an account of the same customer) set an amount and transfer the funds by clicking on the button.

### Implementation details

#### Setup the database context (Bank.Data)
The application will use Entity Framework (EF) to create, manage and access the database. 

The domain classes (the classes that represent entities in our banking domain) can be found in the *DomainClasses* folder. 
In our domain we have 3 classes:
* City: place with a name and a zipcode. The zipcode makes each city unique (primary key).
* Customer: represents a customer of the bank. A customer is linked to a city via the *ZipCode* property (foreign key). The *Name* and *FirstName* of a customer are required properties. One customer can have many accounts.
* Account: banking account of a customer. The *AccountNumber* is required. There are different types of accounts. The type of the account is set by a property of the *AccountType* enum.

**Attention:** the navigation properties (relations) are missing in the domain classes. It is up to you to define the correct relations.

Now turn your attention to the *BankContext* class. 

Add 3 *DbSet*s to the context one for *City*, one for *Customer* and one for *Account*. 
Now EF will know that these domain classes are a part of the database model. 
And thanks to the navigation properties EF will also infer that one customer can have many accounts.

You will still need to help EF a little bit to deduce a correct model from the domain classes. 
Use Fluent API to:
* Set *ZipCode* to be the primary key of a *City*.
* Make the *AcountNumber* of an *Account* required.
* Make the *Name* and *FirtName* properties of a *Customer* required.
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

The *BankContext* will be instantiated at the startup of the application inside *App.xaml.cs*. 
At that moment the *CreateOrUpdateDatabase* method of the context will be called. You need to make sure this method works. 
**Do not change any code inside App.xaml.cs.** You only need to add an implementation to the *CreateOrUpdateDatabase* method.

#### CityRepository.cs (Bank.Data)
The *CityRepository* class implements *ICityRepository* that defines 1 method:
* GetAll: returns all cities in the database

Use the automatic tests for guidance.

#### CustomerRepository.cs (Bank.Data)
The *CustomerRepository* class implements *ICustomerRepository* that defines 3 methods:
* GetAllWithAccounts: returns all customers with there accounts loaded
* Add: adds a new customer to the databse
* Update: updates an existing customer in the database

Use the automatic tests for guidance.

#### AccountRepository.cs (Bank.Data)
The *AccountRepository* class implements *IAccountRepository* that defines 3 methods:
* Add: adds an account to the database.
* Update: updates an existing account in the database. This method should throw an *InvalidOperationException* when the balance of an account is being updated. The balance of an account can only be changed using the *TransferMoney* method.
* TransferMoney: transfers an amount from one account to another account.

Use the automatic tests for guidance.

#### Validators (Bank.Business)
The business layer of the applications contains to validator classes:
* The *CustomerValidator* class is resposible for validating a *Customer* instance. It checks if the first and lastname are filled in and if a valid zipcode is set. 
* The *AccountValidatory* class is resposible for validating an *Account* instance. It checks if an accountnumber is set. Is also verifies that an new account (id <= 0) does not have a negative balance. It also checks if a valid customer and a valid type is set.

Use the automatic tests for guidance.

#### CustomersWindow.xaml (.cs) (Bank.UI)
We want to display the customers in a table layout, we want to be able to edit rows and we want to be able to add new rows (customers). 
There is a (WPF) control that can do all of this stuff and more: the **DataGrid** control. 

Make yourself familiar with this control (their is a big chance that you'll have to work with it on the exam):
* [DataGrid class documentation](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.datagrid)
* [DataGrid tutorial](https://www.wpftutorial.net/DataGrid.html)

In this window the datagrid will have a list of customers (retrieved with an ICustomerRepository) as *ItemsSource*. 
Make sure the correct bindings are used for the different columns. 
For the city combobox column you will need to retrieve a list of all cities and set this list as *ItemsSource* of the column. 
Then you need to make sure that the correct *City* properties are used for the displaytext and value of each item. 
You will also need to tell the datagrid that the selected value in the combobox matches the *ZipCode* property of the customer. 

Provide an implementation for the button click event handlers. 
The unit tests for this class will guide you in the right direction.

Error messages can be shown in the UI using the *ErrorTextBlock*.

Note that the repository classes and validator classes (interfaces) are injected via the constructor.
There is also an *IWindowDialogService* that is injected. Use this object to open the *AccountsWindow* if needed. 
This eliminates the need to new up an instance of *AccountsWindow* directly and makes the class more testable. 
If you are wondering how the customers window is created you can take a look at *App.xaml.cs*.

#### AccountsWindow.xaml (.cs) (Bank.UI)
The UI of the accounts window is very similar to the customers window. 
In this case a datagrid of accounts of the customer is shown. 

Complete the bindings for the datagrid. 
Provide an implementation for the button click event handlers. 
Error messages can be shown in the UI using the *ErrorTextBlock*.

The unit tests for this class will guide you in the right direction.

#### TransferWindow.xaml (.cs) (Bank.UI) 
Provide an implementation for the button click event handler. 
The unit tests for this class will guide you in the right direction.

[img_customers_window]:images/customers_window.png "Overview of customers"
[img_customers_window_editrow]:images/customers_window_edit_row.png "Edit a customer row"
[img_customers_window_addrow]:images/customers_window_add_row.png "Add a new customer row"
[img_customers_window_selectcity]:images/customers_window_select_city.png "Select a city"
[img_accounts_window]:images/accounts_window.png "Overview of the accounts of a customer"
[img_transfer_window]:images/transfer_window.png "Transfer an amount"																				