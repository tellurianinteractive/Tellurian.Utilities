# Tellurian.Utilities
A .NET library with utility functions that tends
to be repeatedly implemented in projects.
Now, they are added in this utility NuGet package
to be included in most projects. 

Most utilities are implemented as extension properties 
and methods and provides short cuts for various 
repeted functionality.

## String Extensions
Namespace `Tellurian.Utilities`

## Date and Time Extensions
Namespace `Tellurian.Utilities`

## Data Extensions
Namespace `Tellurian.Utilities.Data`

Provides extension methods on `IDataRecord` to get values of different types.
These methods has evolved based on experieces 
ot retrieving data from **Microsoft Access**. 
However, the methods can be used for any data
source that supports `IDataRecord`.

## Markdown Extensions
Namespace `Tellurian.Utilities.Web`

Methods for convering markdown to HTML, using
a default `MarkdownPipeline`. This part is based 
om the populular **Markdig** library.

## Color Extensions
Namespace `Tellurian.Utilities.Web`

Methods for handling web colours.

## Http Extensions
Namespace `Tellurian.Utilities.Web`

Extensions on HTTP related objects.


