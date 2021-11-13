# LoxSharp

C# implementation of the lox interpreter described in Crafting interpreters by Robert Nystrom — © 2015 – 2017

## Customizations to the language

### Lambda expressions

```
fun(x) -> x + 1;
```

### Expression if

```
x if x > 0 else -1;
```

### Records instead of classes

Classes cannot have methods, only properties. Classes also have an implicit constructor that receives the properties as named parameters.
e.g.:
```
class Person {
    name: str;
}
fun greet(person) {
    return "Hello " + person.name + "!";
}
greet(Person(name: "Steve"));
```

Thus, there's no `this` (or `self`) and the the following is invalid:
```
class Person {
    name: str;
    fun greet() {
        return "Hello " + this.name + "!";
    }
}
Person(name: "Steve").greet();
```
## To do

Chapter 2:

- [X] Scanning
- [X] Representing Code
- [X] Parsing Expressions
- [X] Evaluating Expressions
- [X] Statements and State
- [X] Control Flow
- [ ] Functions
- [ ] Resolving and Binding
- [ ] Classes (Records)
- [ ] Inheritance (maybe)

Chapter 3:

- [ ] Chunks of Bytecode
- [ ] A Virtual Machine
- [ ] Scanning on Demand
- [ ] Compiling Expressions
- [ ] Types of Values
- [ ] Strings
- [ ] Hash Tables
- [ ] Global Variables
- [ ] Local Variables
- [ ] Jumping Back and Forth
- [ ] Calls and Functions
- [ ] Closures
- [ ] Garbage Collection
- [ ] Classes and Instances
- [ ] Methods and Initializers
- [ ] Superclasses
- [ ] Optimization
