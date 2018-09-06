# Exercises - Chapter 2 - WPF Controls

## Exercise 1
Create a WPF application that resembles the following screenshot:

![alt text][img_exercise1]
 
The first button is a standard button.

The second button should hold an image as well as bold text with a bigger fontsize (16). The image can be found in the *images* folder.
Tip: a *StackPanel* can hold multiple controls and positions them vertically.

The bottom button must have white bold text and a gradient as background (Tip: *LinearGradientBrush*). 
The gradient must move from yellow to red to blue and finally to green with equal distance between the color transitions.

Do all of this **purely in XAML**. Do not change MainWindow.xaml.cs.

## Exercise 2
In this exercise you'll have to apply styles on buttons.
Add a style for buttons to your window resources so you can use it on multiple buttons in the window. 
Add 3 buttons to the window and try to make them resemble the screenshot as good as possible by applying your custom style.
The background of the buttons should be a gradient that flows horizontally. 
It starts with the color *LimeGreen*, is *Green* in the middle and ends with the color *LimeGreen* again.

The middle button should have a bigger font size.

The bottom button should be disabled. Notice how a different background is applied when the button is disabled (this is baked in the *Template* of the *Button* control).

![alt text][img_exercise2]

Do all of this **purely in XAML**. Do not change MainWindow.xaml.cs.

## Exercie 3
Create an application that looks like the image below:

![alt text][img_exercise3]

When the *Grow* button is pressed and the button keeps being pressed down, then the rectangle keeps growing 10 pixels in width.
When the *Shrink* button is pressed and the button keeps being pressed down, then the rectangle keeps shrinking 10 pixels in width.
Position the rectangle in a canvas. 
Make sure the width of the rectangle does not shrink below zero. 
Also make sure the width of the rectangle does not exceed the edge of the canvas. 

Tip: use instances of *RepeatButton*.

## Exercie 4
Create a WPF application that resembles the following screenshot:

![alt text][img_exercise4]

The first button should be a customized *ToggleButton*. When the button is pressed the text on the button changes from "Uit" to "Aan" and visa versa.
Use a *Style* to achieve this.

Add a group of three checkboxes that enables the user to select multiple age groups. Make sure the default selection (when the application starts) matches the selection in the screenshot (checked - not checked - not checked).

Add a group of radiobuttons than enables the user to select a gender. Make sure that "Man" is checked by default (when the application starts).

All controls should be positioned in a *Canvas*.

Do all of this **purely in XAML**. Do not change MainWindow.xaml.cs.

## Exercie 5
Create a WPF application that resembles the following screenshot:

![alt text][img_exercise5]

The necessary images can be found in the *images* folder in the project for this exercise.
Use a *TreeView* control.

Do this **purely in XAML**. Do not change MainWindow.xaml.cs.
 
## Exercie 6
This exercise is about event bubbling and tunneling.

The application (already) contains a button for each digit. When a digit button is pressed the matching digit should be appended in the textbox at the top.
You may only wire up one click event handler (this must be done in XAML) that catches the click event for all buttons.

It is also possible to directly type digits in the textbox. Make sure it is not possible to type non-digit characters in the textbox.
Use event tunneling to achieve this. 
Tip: you can use the static *Char.IsDigit* method to check if a character is a digit.

![alt text][img_exercise6]

## Exercie 7 (TODO)
Creëer een listview met images zoals in volgende screenshot. Zoek gelijkaardige afbeeldingen op het internet.

## Exercie 8 (TODO)
Creëer een button zoals in volgende screenshot. Maak gebruik van een ControlTemplate voor de lay-out van de button. Tip: voorzie 2 Ellipsen binnen elkaar met elk hun eigen LinearGradientBrush)

Wanneer er met de muis over de button wordt bewogen, verandert de kleur van de buitenste ellipse in zwart. Tip: ControlTemplate.Triggers. Trigger Property="Button.IsMouseOver"
Setter Property="Fill"

Bij het klikken op de button, verkleint de button tot 80% (bij het loslaten van de muis vergroot deze weer tot 100% Tip: Trigger Property="Button.IsPressed"
Setter Property="RenderTransform"
 
[img_exercise1]:images/exercise1_mainwindow.png "Main window of exercise 1"
[img_exercise2]:images/exercise2_mainwindow.png "Main window of exercise 2"
[img_exercise3]:images/exercise3_mainwindow.png "Main window of exercise 3"
[img_exercise4]:images/exercise4_mainwindow.png "Main window of exercise 4"
[img_exercise5]:images/exercise5_mainwindow.png "Main window of exercise 5"
[img_exercise6]:images/exercise6_mainwindow.png "Main window of exercise 6"