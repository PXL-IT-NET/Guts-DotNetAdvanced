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

## Exercise 2
Use XAML to create a window that looks as much as possible like this (for different window sizes):

![Exercise2 Main Window Normal](images/Exercise2_MainWindow_normal.png)
![Exercise2 Main Window Wide](images/Exercise2_MainWindow_wide.png)
![Exercise2 Main Window Thin](images/Exercise2_MainWindow_thin.png)

All the *Buttons* in the UI should have some margin on all sides and use the defaults for alignment.
All the *TextBlocks* in the UI should have some margin on all sides, a non-white background color and use the defaults for alignment. 

The *Content* of the *Window* is a *Grid* with 3 rows and 2 columns.
The height of the first row should adjust to the height of its children. 
The other rows should take up the remaining available height. 
The last row should always be twice the height of the middle row.
The first column should always be 100 pixels wide.
The second column should take up the remaining width. 

The grid cells in the first column all contain one button.
The first cell in the second column contains at least 2 stacked buttons.
The second cell in the second column contains at least 2 *TextBlocks* that are positioned from left to right. If there is not enough space the next *TextBlock* is positioned below the others. 
Use a *DockPanel* to position the *TextBlocks* in the last cell of the second row.


[img_mainwindow_normal]:images/MainWindow_normal.png "Normal"
[img_mainwindow_wide]:images/MainWindow_wide.png "Wide"
[img_mainwindow_thin]:images/MainWindow_thin.png "Thin"