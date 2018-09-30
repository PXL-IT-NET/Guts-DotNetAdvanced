# Exercises - Chapter 5 - Unit Testing

## Exercise 2
Fizz buzz is a group word game for children to teach them about division. 
Players take turns to count incrementally, replacing any number divisible by three (the fizz factor) with the word **Fizz**, 
and any number divisible by five (the buzz factor) with the word **Buzz**. 
If the number is divisible by both three and five it should be replaced with the word **FizzBuzz**.

For example, a typical round of fizz buzz would start as follows:

*1 2 Fizz 4 Buzz Fizz 7 8 Fizz Buzz 11 Fizz 13 14 FizzBuzz 16 17 Fizz 19 Buzz Fizz 22 23 Fizz Buzz 26 Fizz 28 29 FizzBuzz 31 32 Fizz 34 Buzz Fizz ...*

The FizzBuzz application you are building can be used to generate the numbers of a FizzBuzz game up until a certain last number (e.g. 100).
The user can also alter the Fizz factor, the Buzz factor and the last number. 

The presentation layer (UI) of the application is already in place. 
It uses a class “FizzBuzzService” in the business layer that can generate the text.

Complete the FizzBuzzService tests (in the test project) and make sure the FizzBuzzService class works correctly.
Complete the following tests:
* ReturnsCorrectFizzBuzzTextWhenParametersAreValid
* ThrowsValidationExceptionWhenFizzFactorIsNotInRange (Minimum = 2, Maximum = 10 -> See constants in *FizzBussService.cs*)
* ThrowsValidationExceptionWhenBuzzFactorIsNotInRange (Minimum = 2, Maximum = 10 -> See constants in *FizzBussService.cs*)
* ThrowsValidationExceptionWhenLastNumberIsNotInRange (Minimum = 1, Maximum = 250 -> See constants in *FizzBussService.cs*)

Make sure to test the following cases in the first test:

| Fizz factor | Buzz factor | Last number | Expected result                                                       |
|-------------|-------------|-------------|-----------------------------------------------------------------------|
| 2           | 3           | 1           | 1                                                                     |
| 4           | 5           | 4           | 1 2 3 Fizz                                                            |
| 5           | 4           | 4           | 1 2 3 Buzz                                                            |
| 2           | 3           | 15          | 1 Fizz Buzz Fizz 5 FizzBuzz 7 Fizz Buzz Fizz 11 FizzBuzz 13 Fizz Buzz |
| 2           | 2           | 4           | 1 FizzBuzz 3 FizzBuzz                                                 |

Write the test first and then try to make it green by altering the production code.
The names of the tests should be enough info to know what they should do.
Use a setup method.

The tests in the project **Guts.Tests** will check your solution. **Do not modify these tests!**