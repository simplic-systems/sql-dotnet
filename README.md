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

## Debugger

We are also creating some debugger tool. The debugger is not directly for debugging SQL-Statements, but for debugging the Compiler
and runtime, because it visualize all generated results.

![Debugger-Screen](/img/Debugger.png)

### Tokenizer

The tokenizer takes the input sql code and makes build tokens. E.g.: `SELECT A FROM B` will be splitted into `<SELECT> <A> <FROM> <B>`.

*Involved files*

1. */Compiler/Tokenizer/Tokenizer.cs* Tokenizer core
2. */Compiler/Tokenizer/ParserConfiguration.cs* Configuration files for token generating

### Syntax-Tree-Builder (AST)

The syntaxtree builder classifies all token in the `Build` method and creates the `Abstract-Syntax-Tree (AST)`. In this steps first Sql
sematic analysis will be done. The result is a tree build of `SyntaxTreeNodes`. Also conditions and other expressions will be resolved
using the `Shuntig-Yard` algorithm.

*Involved files*

1. */Compiler/SyntaxTree/SyntaxTreeBuilder.cs* Main tree builder
2. */Compiler/SyntaxTree/Nodes/\** All node definitions
1. */Compiler/SyntaxTree/Factory/SyntaxTreeFactory.cs* Token classification
3. */Compiler/SyntaxTree/ShuntingYard.cs* Shunting Yard algorithm

### Sematic-Analysis

_Not yet implemented_

### SIQL-Generator/Compiler

Takes the optimized and generated `AST` and compiles it to `SIQL` code.

1. */Compiler/ILCompiler/SIQLCompiler.cs* Main compiler

## Runtime

The `runtime` is responsible to execute `SIQL` code and call the `IQueryExecutor` interface/implementation.
