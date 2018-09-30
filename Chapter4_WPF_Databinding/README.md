# Exercises - Chapter 4 - WPF Databinding

## Exercise 1
Use *OneWay* databinding to show a game object on the screen. 

The UI (XAML) is given. You only have to add the bindings.

The *Game* class is also given. And the constructor of the *MainWindow* already creates a game object.

## Exercise 2
In this exercise you will also visualise game objects in the UI.
This time it is just a little bit more complicated.

The *Game* class is given and contains a few more properties compared to exercise 1.

The user can select 1 of 7 games in a ComboBox. The properties of the selected game are displayed in the UI. 
It is also possible to change the rating of a game by clicking on the *Up* or *Down* button. 
All changes in the UI (e.g. change the name of a game) should be reflected in the game object the UI is showing.

The UI (XAML) is (partially) given. You have to add the bindings and make sure the games are displayed correctly in the combobox. 
In the combobox each item is displayed as *Game Id - Name* (Use a *StackPanel* and databinding to achieve this).

The ratings of the game objects in memory are doubles between 0.0 and 10.0. 
Use a converter to display the ratings in the UI as doubles between 0.0 and 100.0. 
The converter will need to work in 2 directions. 
The (empty) converter class *RatingConverter* is already present in the code (*Converters\RatingConverter.cs*). 