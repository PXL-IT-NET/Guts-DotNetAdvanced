# Exercises - Chapter 7 - Layered Architecture

## Exercise 1 (Heroes)
In this exercise you will be creating a desktop application 
that randomly creates a battle between 2 heroes. 
The 2 heroes attack each other round by round. 
When the health of one of the heroes reaches zero the fight ends and the button is disabled.

![Hero App](images/heroApp.png)

When one hero attacs another hero, the attack strength (1-100) determines the damage on the health of the other hero. 
Each hero has a *supermode likeliness*. This is the chance that the attack of a hero does double damage or a defend against an attack halfs the damage received. 
In each round figher 1 attacks fighter 2 and fighter 2 then attacks fighter 1.
Changes in health should be reflected on screen.

Your solution should be organized in a proper layered structure. 

### Domain
Start with the domain layer. This layer already exposes some public interfaces (in the *Contracts* folder). 
You must create the concrete implementations and hide these implementations from the other layers (so that they are forced to use the public interfaces).
Other layers must use the factory interfaces to create instances of *Hero* or *Battle*.

Use the automated tests to guide you in the right direction.

Tip for the factory implementations:

The concrete implementation of e.g. the *IHeroFactory* will need access to the (private) constructor of the *Hero* class. 
You can achieve this by nesting a *Factory* class inside the *Hero* class definition. 
See [https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/nested-types](https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/nested-types).

### Business
When the domain layer is in place, implement the business layer. 

Use the automated tests to guide you in the right direction.

### Data
When the business layer is in place, implement the data layer.

Use the automated tests to guide you in the right direction.

### Presentation
When the data layer is in place, implement the presentation (UI) layer.

Use the automated tests to guide you in the right direction.


