# Exercises - Chapter 10 - ADO.NET Transactions

## Exercise 1

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

#### Create the database
Connect with *(localdb)\MSSQLLocalDB* and exectute [scripts/CreateAndFillBankDB.sql](scripts/CreateAndFillBankDB.sql). 
This scripts creates the *BankDB_ADO* database and fills it with some data.

#### ConnectionFactory.cs (Bank.Data)
The *ConnectionFactory* class is resposible for creating instances of *SqlConnection*. 
It implements the interface *IConnectionFactory*. Classes (repositories) that need to have a database connection will use an implementation of this interface to create the connection.  
Make sure the *SqlConnection* uses the *BankConnection* connectionstring in the *App.config* (of the Bank.UI project).

**Tip**: categorize your tests in *Test Explorer* by *Class*. If you right click on a category (class) only tests for that class are runned.

#### CustomerRepository.cs (Bank.Data)
The *CustomerRepository* class implements *ICustomerRepository* that defines 3 methods:
* GetAll: returns all customers from the database
* Add: adds a new customer to the databse
* Update: updates an existing customer in the database

Use ADO.NET to implement these methods. 
The unit tests for this class will help you build a solid errorprone repository.

Note that an *IConnectionFactory* is injected in the repository via a constructor parameter. 
Use this object to create a connection to the database. 

#### CityRepository.cs (Bank.Data)
The *CityRepository* class implements *ICityRepository* that defines 1 method:
* GetAll: returns all cities in the database

Use ADO.NET to implement this method. 
The unit test for this class will check if you implemented the method correctly.

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

Note that the repository classes (interfaces) are injected via the constructor.
There is also an *IWindowDialogService* that is injected. Use this object to open the *AccountsWindow* if needed. 
This eliminates the need to new up an instance of *AccountsWindow* directly and makes the class more testable. 
If you are wondering how the customers window is created you can take a look at *App.xaml.cs*.

#### AccountRepository.cs (Bank.Data)
The *AccountRepository* class implements *IAccountRepository* that defines 4 methods:
* GetAllAccountsOfCustomer: returns all the accounts linked to a certain customer
* Add: adds a new account for a customer to the databse
* Update: updates an existing account in the database
* TransferMoney: transfer money between two accounts of the same customer. 

Use ADO.NET to implement these methods. 
The TransferMoney method should use a transaction to avoid money getting lost or created.
The unit tests for this class will help you build a solid errorprone repository.

#### AccountsWindow.xaml (.cs) (Bank.UI)
The UI of the accounts window is very similar to the customers window. 
In this case a datagrid of accounts of the customer is shown.
You can edit accounts but not the balance. To change the balance you will have to do it via the *Transfer amount* button that opens the transfer window. 

Complete the bindings for the datagrid. 
Provide an implementation for the button click event handlers. 
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