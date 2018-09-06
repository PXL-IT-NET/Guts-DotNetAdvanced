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

## Exercie 4 (TODO)
Creëer volgende WPF applicatie:
 
Wanneer er op de schakelaar wordt geklikt, verandert de tekst op de button naar Uit en omgekeerd. Gebruikt hiervoor opnieuw een Style (tips: ToggleButton, Style.Triggers)
Voorzie 3 checkboxes voor de leeftijd. Stel de selectie in zoals op de schermafbeelding (True – False – False)
Voorzie 2 radiobuttons voor het geslacht. Stel de selectie in zoals op de schermafbeelding (True – False)
Plaats alle controls op een Canvas.

## Exercie 5 (TODO)
Creëer volgende WPF applicatie
 
Zoek de afbeeldingen zelf op internet. Je mag deze boomstructuur aanvullen met landen / landen uit andere werelddelen.

## Exercie 6 (TODO)
Creëer een WPF applicatie zoals in volgende screenshot. Zorg dat er in de textboxes GEEN cijfers mogen ingegeven worden (test alleen op cijfers). Toon een messagebox (“Enkel karakters toegelaten”) wanneer er getracht wordt om cijfers in te geven.
Tips: event routing, char.IsDigit

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