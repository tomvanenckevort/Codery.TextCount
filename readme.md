Codery.TextCount
=====================
An Umbraco data type which wraps existing text data type controls and shows text counts and limits while you type. 

How to use it
-------------
Create a new data type using the 'Text Count' property editor.

Next up, choose your options:

- _Data Type_: choose the existing text-based data type that you want to add a counter to (supported types: Textstring, Textarea, RTE, Markdown and Repeatable Textstring).
- _Characters or Word_: choose whether the counter should count individual characters or whole words.
- _Limit_: set the number of characters or words you want to be the limit (leave empty if there is no limit).
- _Limit Type_: choose whether the limit is a warning (but allows users to add more), a hard warning (preventing users to save more than the specified limit) or no limit (the text counter just acts as a guide).

Save it, add it to a document type and your editors will now see the text counter displayed next to the input field.

How to install it
-----------------
The package can be installed through NuGet:
```
Install-Package Codery.TextCount 
```
