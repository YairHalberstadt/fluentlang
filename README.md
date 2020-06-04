# FluentLang

[![Build Status](https://dev.azure.com/yairhalberstadt/FluentLang/_apis/build/status/YairHalberstadt.fluentlang?branchName=master)](https://dev.azure.com/yairhalberstadt/FluentLang/_build/latest?definitionId=3&branchName=master)

FluentLang is an experimental programming language designed to explore the intersection between structural typing, object oriented programming, and functional programming.

Currently it is very much a work in progress.

## Building

Command Line: Make sure dotnet core 3.1 is installed. Run `dotnet build` to build, and `dotnet test` to run tests.

IDE: Open in VS 2019 > version 4. Everything should work as normal.

## Aims

This language is primarily a research language. It is as far as I know completely novel in its design, and is intended as a project to research how feasible the design is in practice.

As such the language prioritizes simplicity of implementation and definition, over features and ease of use. As such it may be a bit awkward to use in practice.

At the same time I do try to keep in mind what syntax sugar and extra features I might want to add, and how they interact with the current design, when designing the language.

Performance is not a priority for this language, but at the same time I am trying to keep performance within reason. For that reason certain features (such as including method subtyping in interface subtyping) have been put off till a practical performant design can be worked out for them.

## Concepts

### Types

A type is either an interface or a primitive.

#### Interfaces

An interface is a collection of methods, which may or may not be named. All interfaces are structurally typed. An interface a is equivalent to an interface b, if for every method a defines, b defines an equivalent method, and vice versa. Two methods are equivalent if every parameter type and the return type are equivalent. 

An interface a is a subtype of another interface b, if for every method in b, a defines an equivalent method.

If an interface a being equivalent to an interface b would not break this definition, they are considered equivalent, even if it is not required for them to be equivalent to avoid breaking the definition. For example:

```
interface a
{
    M() : a;
}

interface b
{
    M() : b;
}
```

Here a and b are equivalent.

Every object has an implicit interface it implements, and is a subtype of any interface this interface is a subtype of.

#### Primitives

There are currently 5 primitives defined:

bool

int

double

char

string

This is sufficient for 99% of use cases in my experience. All of these are defined to have the same structure as their C# equivalents.

The primitives implement no interfaces, and cannot be boxed.

A primitive is only equivalent to itself, and only a subtype of itself.

### Methods

Methods take a list of named and typed parameters and return a typed object.

To call a method requires passing in a list of arguments of the same length as the parameters of the method. All the arguments must be statically typed as subtypes of their respective parameter.

Methods can define local methods and interfaces, which are scoped to that method, and it's subMethods/subInterfaces.

Local methods can capture variables. Since all variables are immutable, whether they are captured by value or reference is an implementation detail.

### Objects

There are no such thing as classes in FluentLang.

The only object that can be created is the empty object `{}`, which implements the empty interface.

It is then possible to bind methods to the object.

```
M1(param : {}) : {}
{
    return param;
}

M2(param : {}) : int
{
    return 5;
}

...

bound = {} + M1 + M2;

resultM1 = bound.M1();
resultM2 = bound.M2();
```

binding creates a new object with the bound methods attached. This new object implements the union of its original interface, and the interface containing the bound methods with their first parameter removed.

It is only possible to bind methods where the type of the returned object from the binding expression is a subtype of the first parameter of the method. For example:

```
interface Counter
{
    Increment() : Counter;
    Value() : int;
}

CreateCounter() : Counter
{
      return {} + Increment, Value;
      Increment(counter : Counter) : Counter
      {
          value = counter.Value();
          return counter + Value;
          Value(counter : {}) : int
          {
              return value + 1;
          }
      }

      Value(counter : {}) : int
      {
          return 0;
      }
}
```

### Generics

There are two forms of generics in fluentlang:

**Generic Named Interfaces**
When an interface is declared it may specify some type parameters. Then when it is used, type arguments must be supplied for all type parameters, making the interface concrete.

E.g:

```
interface Factory<T>
{
    Create() : T;
}

M(intFactory : Factory<int>) : int { return intFactory.Create(); }
```

**Generic Methods**
A method (but not an interface method) may be generic.

E.g.

```
interface Factory<T>
{
    Create() : T;
}

CreateViaFactory<T>(factory : Factory<T>) : T { return factory.Create(); }
```

### Unions

It is possible to specify that a type must match at least one of a number of options via union types:

E.g. `int | { M1() : int; } | { M2() : bool; }`

It is then possible to match on the type to specify different behaviour based on which of the options it is. 

E.g.

```
Main() : int {
	let u : int | {} = 42;
	return u match { x : int => x; {} => 41; }; // returns 42
}
```

For more information see https://github.com/YairHalberstadt/fluentlang/issues/6

### Immutability

Pure FluentLang is purely immutable. There is no way to change any existing variable. As a result pure FluentLang is also referentially transparent.

Howevers operations such as IO are performed via interop with C# libraries. There is no guarantee as to the behaviour of these libraries.

### Object Oriented Programming

Objects have no fields, only methods. Objects are not instances of a class, but rather are produced in prototypical fashion, by copying and adding methods to existing objects. From that perspective they share a lot in common with Alan Kay's perspective on object oriented programming, although there are of course many differences as well.

## Status

### Completed:

ANTLR4 lexer and parser

Set up CI pipeline on Azure

Creation of Symbol API

Semantic checks (type checking etc.)

Implement runtime

Implement Emitting of metadata

Implement Reading of Metadata

Implement Emitting of code

Implement Linking

Create compiler.exe

Design and implement unions. See #6, #8.

Design and implement generics. See #9, #10.

### In progress

Implement Standard Libraries. This will be mixed C# Libraries with FluentLang Metadata and FluentLang libraries.

Create Blazor WebIDE.

### ToDo

Look into more efficient incremental compilation.

## Contributing

Projects which should be possible for someone else to do with us stepping on each others toes:

1. Look into more efficient incremental compilation. Can we avoid recompiling everything whenever a single file in a single dependency changes?
1. Improve syntax error recovery. Currently we discard the entire file if it contains a syntax error. Thats a really bad strategy for an IDE.
1. Look into APIs for code completion, syntax highlighting, etc.
1. Come up with design proposals for: Generic interface methods, method subtyping, error handling (exceptions?, result types?)
    - Design proposals should be created as issues on this repository with the heading "Proposal:". E.g. "Proposal: design for Method Subtyping".
1. [Improve diagnostic messages](https://github.com/YairHalberstadt/fluentlang/issues/7)
1. Improve Blazor WebIDE. My web design skills are extremely primitive. Any help here would be greatly appreciated.
