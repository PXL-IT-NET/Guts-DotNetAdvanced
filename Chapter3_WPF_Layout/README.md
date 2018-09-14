# Exercises - Chapter 3 - WPF Layout

## Exercise 1
Use XAML to create a window that looks as much as possible like this (for different window sizes):

![alt text][img_mainwindow_normal] 
![alt text][img_mainwindow_wide] 
![alt text][img_mainwindow_thin]
 
Use a *DockPanel* to display a *Grid* on the left and a button on the right.
The right *Button* should hold tree lines of text (each line in a *TextBlock*). 
Make sure there is some margin between each *TextBlock* on all sides (e.g. *Margin=4*). 
The background color of the *TextBlock* elements in the screenshot is *LightCyan*.

The *Grid* on the left has 4 cells in 2 rows and 2 columns. 
The heights of the grid rows should adjust to the height available in the parent element. 
The second row should always be 3 times higher than the first row.
The width of the first column should be an absolute amount. E.g. 100 pixels.
The with of the second column should take up the remaining space.

Both cells in the first row of the *Grid* should contain a button. 
The first cell in the second row also contains a button, but here the button is not stretched vertically. 
All other buttons in the *Window* should have their alignment set to stretch (which is the default). 
The second cell in the second row contains a *WrapPanel* that should have at least 5 *Button* controls. 
Add some padding on all sides of each button in this panel (e.g. *Padding="10"*)

Run the application and see what happens when you resize the window. 
Make sure you understand why WPF arranges the controls the way you see. 
See what happens when you use *Auto* for the width of the second column of the *Grid*.

[img_mainwindow_normal]:images/MainWindow_normal.png "Normal"
[img_mainwindow_wide]:images/MainWindow_wide.png "Wide"
[img_mainwindow_thin]:images/MainWindow_thin.png "Thin"