SQL-DotNet
===

Sql dot is a library which allows, to embed your own sql engine (parser, compiler and runtime) into your .net application.
The parser is completly written using `c#` and available under the `MIT` license.

Initialy the parser is written for the `ODBC-Driver` used by the `Simplic Content Delivery Network` and is set as open source
project, to let other benefit from this project and create some great sql-implementation togehter.

## Aim

The aim is, to create a library which allows to embed sql in your appliaction, only by implementing a single interface: `IQueryExecutor`.
The complete communication with your software is written in this implementation and registered in one line.

*Implementation*

```csharp
public class MyQueryExecutor : IQueryExecutor
{
    // ...
}
```

*Registration*

```csharp
// Register the query executor
SqlDotNet.Sql.RegisterExecutor<MyQueryExecutor>()
```

## General things to know

### SIQL

SIQL - Structured Intermidiate Query Lanugage. All sql queries will be compiled into this simple language before execution.
This language is similar to `.net IL` code and other intermedate languages.

### Library structure

The library is structured into a `compiler` and `runtime`. Both with be explained in this article, to get an overview.
The library is under heavy development, so changes are maybe not up-to-date here.

## Compiler pipeline

The `compiler` compiles a sql query into `SIQL` code and cache it. If the same query will be executed later, it has not to be compiled
again. The compiler is splitted into the following steps.

### Tokenizer

### Syntax-Tree-Builder (AST)

### Sematic-Analysis

### SIQL-Generator/Compiler

## Runtime

The `runtime` is responsible to execute `SIQL` code and call the `IQueryExefutor` interface/implementation.
