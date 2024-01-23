# Coding Standards C\#

🏠 [Go back to README.md](/README.md)

## Naming Conventions

“c” = camelCase

“P” = PascalCase

“_” = Prefix with \_Underscore

“ ” = Not Applicable.

| **Identifier**         | **Public** | **Protected** | **Internal** | **Private** | **Notes**                                         |
|------------------------|------------|---------------|--------------|-------------|---------------------------------------------------|
| Project File           | **P**      |               |              |             | *Match Assembly & Namespace.*                     |
| Source File            | **P**      |               |              |             | *Match contained class.*                          |
| Other Files            | **P**      |               |              |             | *Apply where possible*                            |
| Namespace              | **P**      |               |              |             | *Partial Project/Assembly match.*                 |
| Class or Struct        | **P**      | **P**         | **P**        | **P**       | *Add suffix of subclass*                          |
| Interface              | **P**      | **P**         | **P**        | **P**       | *Prefix with a capital I.*                        |
| Generic Class [C\#v2+] | **P**      | **P**         | **P**        | **P**       | *Use T or K as Type identifier.*                  |
| Method                 | **P**      | **P**         | **P**        | **P**       | *Use a Verb or Verb-Object pair.*                 |
| Property               | **P**      | **P**         | **P**        | **P**       | *Do not prefix with Get or Set.*                  |
| Field                  | **P**      | **P**         | **P**        | **\_c**     | *Only use Private fields. No Hungarian Notation!* |
| Constant               | **P**      | **P**         | **P**        | **\_c**     |                                                   |
| Static Field           | **P**      | **P**         | **P**        | **\_c**     | *Only use Private fields*                         |
| Enum                   | **P**      | **P**         | **P**        | **P**       | *Options are also PascalCase*                     |
| Delegate               | **P**      | **P**         | **P**        | **P**       |                                                   |
| Delegate               | **P**      | **P**         | **P**        | **P**       |                                                   |
| Event                  | **P**      | **P**         | **P**        | **P**       |                                                   |
| Inline Variable        |            |               |              | **c**       | *Avoid single-character and enumerated names.*    |
| Parameter              |            |               |              | **c**       |                                                   |

## Coding Style

- Source Files One Namespace per file and one class per file.

- Curly Braces On new line. Always use braces when optional.

- Indention Use tabs with size of 4.

- Comments Use `//` or `///` but not `\*` … `*/` and do not flowerbox.

- Variables One variable per declaration.

- Use postfix `Async` for async methods.

    **Example**:

    ```csharp
    // Bad!
    private async Task Handle() 
    {…}

    // Good!
    private async Task HandleAsync() 
    {…}
    ```

## Formatting

1. Never declare more than 1 namespace per file.

2. Avoid putting multiple classes in a single file.

3. Always place curly braces (**{** and **}**) on a new line.

4. Always use curly braces (**{** and **}**) in conditional statements.

5. Always use a Tab.

6. Declare each variable independently – not in the same statement.

7. Place namespace without brackets `{}`use `;` instead.

    **Example**:

    ```csharp
    // Bad!
    namespace YourNameSpace 
    {
        using AutoMapper;
        using MediatR;
        using System.Threading.Tasks;

        {…}
    }

    // Good!
    namespace YourNameSpace;

    using AutoMapper;
    using MediatR;
    using System.Threading.Tasks;
    ```

8. Place namespace “**using**” statements together at the top of file. Group .NET namespaces below custom namespaces.

    **Example**:

    ```csharp
    // Bad!
    using AutoMapper;
    using MediatR;
    using System.Threading.Tasks;

    namespace YourNameSpace;    
    {…}

    // Good!
    namespace YourNameSpace;

    using AutoMapper;
    using MediatR;
    using System.Threading.Tasks;
    ```

9. Group internal class implementation by type in the following order:

    1. Member variables.

    2. Properties

    3. Nested Enums, Structs, and Classes.

    4. Constructors & Finalizers.

    5. Methods

10. Sequence declarations within type groups based upon access modifier and
    visibility:

    1. Public

    2. Protected

    3. Internal

    4. Private

11. Append folder-name to namespace for source files within sub-folders.

12. Recursively indent all code blocks contained within braces.

13. Use white space (CR/LF, Tabs, etc) liberally to separate and organize code.

14. Only declare related **attribute** declarations on a single line, otherwise stack each attribute as a separate declaration.

    **Example**:

    ```csharp
    // Bad!
    [Attrbute1, Attrbute2, Attrbute3]
    public class MyClass
    {…}

    // Good!
    C#
    [Attrbute1, RelatedAttribute2]
    [Attrbute3]
    [Attrbute4]
    public class MyClass
    {…}
    ```

15. Place Assembly scope **attribute** declarations on a separate line.

16. Place Type scope **attribute** declarations on a separate line.

17. Place Method scope **attribute** declarations on a separate line.

18. Place Member scope **attribute** declarations on a separate line.

19. Place Parameter **attribute** declarations inline with the parameter.

20. If in doubt, always err on the side of clarity and consistency.

## Code Commenting

1. All comments should be written in the same language, be grammatically correct, and contain appropriate punctuation.

2. Use `//` or `///` but **never** `\*` … `*/`.

3. Do not “flowerbox” comment blocks.

    **Example:**

    ```CSharp
    // ***************************************
    // Comment block
    // ***************************************
    ```

4. Use inline-comments to explain assumptions, known issues, and algorithm insights.

5. Do not use inline-comments to explain obvious code. Well written code is self documenting.

6. Only use comments for bad code to say “fix this code” – otherwise remove, or rewrite the code!

7. Include comments using Task-List keyword flags to allow comment-filtering.

    **Example:**

    ```csharp
    // TODO: Place Database Code Here
    // UNDONE: Removed P\Invoke Call due to errors
    // HACK: Temporary fix until able to refactor
    ```

8. Always apply C\# comment-blocks (`///`) to **public**, **protected**, and **internal** declarations. Only when we must use for documenting the API.

9. Always include **\<summary\>** comments. Include **\<param\>**, **\<return\>**, and **\<exception\>** comment sections where applicable.

10. Include **\<see cref=””/\>** and **\<seeAlso cref=””/\>** where possible.

11. Always add CDATA tags to comments containing code and other embedded markup in order to avoid encoding issues.

    **Example:**

    ```csharp
    /// <example>
    /// Add the following key to the “appSettings” section of your config:
    /// <code><![CDATA[
    ///     <configuration>
    ///          <appSettings>
    ///               <add key=”mySetting” value=”myValue”/>
    ///          </appSettings>
    ///     </configuration>
    /// ]]></code>
    /// </example>
    ```

## Language Usage

### General

1. Do not omit access modifiers. Explicitly declare all identifiers with the appropriate access modifier instead of allowing the default.

    **Example**:

    ```csharp
    // Bad! 
    void WriteEvent(string message)
    {…}

    // Good! 
    private void WriteEvent(string message) 
    {…}
    ```

2. Avoid mutual references between assemblies.

## Variables & Types

1. Try to initialize variables where you declare them.

2. Use always `var` instead of the type.

    ```csharp
    // Bad! 
    string @value = "hi world!";
    {…}

    // Good! 
    var @value = "hi world!";
    {…}
    ```

3. Always choose the simplest data type, list, or object required. For example if you want to read a collection of some ids, and you don't need to keep ordered use the interface `IEnumerable` instead of List. This apply for all cases like `Dictionary` / `IDictionary`, etc.

    **Example**

    ```csharp
    List<int> ids; <-> IEnumerable<int> ids; <-> int[] ids;
    Dictionary<int> ids; <-> IDictionary<int> ids;
    {…}
    ```

4. Always use the built-in C\# data type aliases, not the .NET common type system (CTS).

    **Example**:

    ```csharp
    short NOT System.Int16 
    int NOT System.Int32 
    long NOT System.Int64 
    string NOT System.String
    ```

5. Only declare member variables as **private**. Use properties to provide access to them with **public**, **protected**, or **internal** access modifiers.

6. Try to use **int** for any non-fractional numeric values that will fit the **int** datatype - even variables for nonnegative numbers.

7. Only use **long** for variables potentially containing values too large for an **int**.

8. Try to use **double** for fractional numbers to ensure decimal precision in calculations.

9. Only use **float** for fractional numbers that will not fit **double** or **decimal**.

10. Avoid using **float** unless you fully understand the implications upon any calculations.

11. Try to use **decimal** when fractional numbers must be rounded to a fixed precision for calculations. Typically this will involve money.

12. Avoid using **sbyte**, **short**, **uint**, and **ulong** unless it is for interop (P/Invoke) with native libraries.

13. Avoid specifying the type for an **enum** - use the default of **int** unless you have an explicit need for **long** (very uncommon).

14. Avoid using inline numeric literals (magic numbers). Instead, use a **Constant** or **Enum**.

15. Avoid declaring string literals inline. Instead use Resources, Constants, Configuration Files, Registry or other data sources.

16. Declare **readonly** or **static readonly** variables instead of constants for complex types.

17. Only declare **constants** for simple types.

18. Avoid direct casts. Instead, use the “**as**” operator and check for **null**.

    **Example**:

    ```csharp
    var dataObject = LoadData(); 
    var ds = dataObject as DataSet; 
    if (ds != null) 
    {…}
    ```

19. Always prefer C\# Generic collection types over standard or strong-typed collections. [C\#v2+]

20. Always explicitly initialize arrays of reference types using a for loop.

21. Avoid boxing and unboxing value types.

    **Example**:

    ```csharp
    var count = 1; 
    object refCount = count; // Implicitly boxed. 
    var newCount = (int)refCount; // Explicitly unboxed.
    ```

22. Floating point values should include at least one digit before the decimal place and one after.

    **Example**: totalPercent = 0.05;

23. Try to use the “**@**” prefix for string literals instead of escaped strings.

24. Prefer **String.Format()** or **StringBuilder** over string concatenation.

25. Concatenate using [string interpolation](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated) to concatenate strings:

    **Example**:

    ```csharp
    var name = "Mark";
    var date = DateTime.Now;

    // String interpolation:
    var text = $"Hello, {name}! Today is {date.DayOfWeek}.";
    ```

26. Never concatenate strings inside a loop.

27. Do not compare strings to **String.Empty** or **“”** to check for empty strings. Instead, compare by using **String.Length == 0**.

28. Avoid hidden string allocations within a loop. Use **String.Compare()** for case-sensitive

    **Example**: *(ToLower() creates a temp string)*

    ```csharp
    // Bad! 
    var id = -1; 
    var name = “lance hunt”;
    for (int i=0; i < customerList.Count; i++) 
    { 
        if (customerList[i].Name.ToLower() == name) 
        {
            id = customerList[i].ID; 
        } 
    } 

    // Good!
    var id = -1;
    var name = “lance hunt”;

    for(var i=0; i < customerList.Count; i++) 
    {
        // The “ignoreCase = true” argument performs a 
        // case-insensitive compare without new allocation.
        if (String.Compare(customerList[i].Name, name, true)== 0) 
        { 
            id = customerList[i].ID; 
        } 
    }
    ```

### Flow Control

1. Avoid invoking methods within a conditional expression.

2. Avoid creating recursive methods. Use loops or nested loops instead.

3. Avoid using **foreach** to iterate over immutable value-type collections. E.g. String arrays.

4. Do not modify enumerated items within a **foreach** statement.

5. Use the **ternary** conditional operator only for trivial conditions. Avoid complex or compound ternary operations.

    **Example**:

    ```csharp
    var result = isValid ? 9 : 4;
    ```

6. Evaluate Boolean conditions against false.

    **Example**:

    ```csharp
    // Bad! 
    if (!isValid) 
    {…} 

    // Good! 
    if (isValid == false) 
    {…} 
    ```

7. Avoid assignment within conditional statements.

    **Example**:

    ```csharp
    if ((i=2)==2) {…}
    ```

8. Avoid compound conditional expressions – extract to a new method.

    **Example**:

    ```csharp
    // Bad! 
    if (((value > _highScore) && (value != _highScore)) && (value < _maxScore)) 
    {…} 

    // Good!     
    if (ValidScore(value))
    {…} 
        
    private bool ValidScore(int value) 
        => ((value > _highScore) && (value != _highScore)) && (value < _maxScore)}
    ```

9. Avoid explicit Boolean tests in conditionals.

    **Example**:

    ```csharp
    // Bad! 
    if(IsValid == true) 
    {…}; 

    // Good! 
    if(IsValid) 
    {…}
    ```

10. Avoid use **switch/case** statements.

11. Avoid use **else** statements.

### Exceptions

1. Do not use **try/catch** blocks for flow-control.

2. Only **catch** exceptions that you can handle.

3. Never declare an empty **catch** block.

4. Avoid nesting a **try/catch**  within a **catch** block.

5. Always catch the most derived exception via exception filters.

6. Order exception filters from most to least derived exception type.

7. Avoid re-throwing an exception. Allow it to bubble-up instead.

8. If re-throwing an exception, preserve the original call stack by omitting the exception argument from the **throw** statement.

    **Example**:

    ```csharp
    // Bad! 
    catch(Exception ex) 
    { 
        Log(ex); 
        throw ex; 
    } 

    // Good! 
    catch(Exception) 
    { 
        Log(ex);
        throw; 
    }
    ```

9. Only use the **finally** block to release resources from a **try** statement.

10. Always use validation to avoid exceptions.

    **Example**:

    ```csharp
    // Bad! 
    try 
    { 
        conn.Close(); 
    } 
    Catch(Exception ex) 
    { 
        // handle exception if already closed! 
    } 

    // Good! 
    if(conn.State != ConnectionState.Closed) 
    { 
        conn.Close(); 
    }
    ```

11. Always set the **innerException** property on thrown exceptions so the exception chain & call stack are maintained.

12. Avoid defining custom exception classes. Use existing exception classes instead.

13. When a custom exception is required;

    1. Always derive from **Exception not ApplicationException**.

    2. Always suffix exception class names with the word “Exception”.

    3. Always add the **SerializableAttribute** to exception classes.

    4. Always implement the standard “Exception Constructor Pattern”:

    ```csharp
    public MyCustomException (); 
    public MyCustomException (string message); 
    public MyCustomException (string message, Exception innerException);
    ```

14. Always implement the deserialization constructor:

    ```csharp
    protected MyCustomException(SerializationInfo info, StreamingContext contxt); 
    ```

15. When defining custom exception classes that contain additional properties:

    1. Always override the **Message** property, **ToString**() method and the **implicit operator string** to include custom property values.

    2. Always modify the deserialization constructor to retrieve custom property values.

    3. Always override the **GetObjectData(…)** method to add custom properties to the serialization collection.

        **Example**:

        ```csharp
        public override void GetObjectData(SerializationInfo info, StreamingContext context) 
        { 
            base.GetObjectData (info, context); info.AddValue("MyValue", _myValue); 
        }
        ```

### Events, Delegates, & Threading

1. Always check Event & Delegate instances for **null** before invoking.

2. Use the default **EventHandler** and **EventArgs** for most simple events.

3. Always derive a custom **EventArgs** class to provide additional data.

4. Use the existing **CancelEventArgs** class to allow the event subscriber to
    control events.

5. Always use the “**lock**” keyword instead of the **Monitor** type.

6. Only lock on a private or private static object.

    **Example**: **lock**(myVariable);

7. Avoid locking on a Type.

    **Example**: **lock**(**typeof**(**MyClass**));

8. Avoid locking on the current object instance.

    **Example**: **lock**(**this**);

### Object Composition

1. Always declare types explicitly within a namespace. Do not use the default “{global}” namespace.

2. Avoid overuse of the **public** access modifier. Typically fewer than 10% of your types and members will be part of a public API, unless you are writing a class library.

3. Consider using **internal** or **private** access modifiers for types and members unless you intend to support them as part of a public API.

4. Never use the **protected** access modifier within **sealed** classes unless overriding a **protected** member of an inherited type.

5. Avoid declaring methods with more than **5** parameters. Consider refactoring this code.

6. Try to replace large parameter-sets (\> than **5** parameters) with one or more **class** or **struct** parameters – especially when used in multiple method signatures.

7. Do not use the “**new**” keyword on method and property declarations to hide members of a derived type.

8. Only use the “**base**” keyword when invoking a base class constructor or base implementation within an override.

9. Consider using method overloading instead of the **params** attribute (but be careful not to break CLS Compliance of your API’s).

10. Always validate an enumeration variable or parameter value before consuming it. They may contain any value that the underlying Enum type (default int) supports.

    **Example**:

    ```csharp
    public void Test(BookCategory cat) 
    { 
        if (Enum.IsDefined(typeof(BookCategory), cat)) 
        {…} 
    } 
    ```

11. Consider overriding **Equals()** on a **struct**.

12. Always override the **Equality Operator** (**==**) when overriding the **Equals()** method.

13. Always override the **String Implicit Operator** when overriding the **ToString()** method.

14. Always call **Close()** or **Dispose()** on classes that offer it.

15. Wrap instantiation of **IDisposable** objects with a “**using**” statement to ensure that **Dispose()** is automatically called.

    **Example**:

    ```csharp
    using(SqlConnection cn = new SqlConnection(_connectionString)) 
    {…} 
    ```

16. Always implement the **IDisposable** interface & pattern on classes referencing external resources.

    **Example**: *(shown with optional Finalizer)*

    ```csharp
    public void Dispose() 
    { 
        Dispose(true); 
        GC.SuppressFinalize(this); 
    } 

    protected virtual void Dispose(bool disposing) 
    { 
        if (disposing) 
        { 
            // Free other state (managed objects). 
        } 
        // Free your own state (unmanaged objects). 
        // Set large fields to null. 
    } 

    // C# finalizer. (optional) 
    ~Base() 
    { 
        Simply call Dispose(false). 
        Dispose (false); 
    } 
    ```

17. Avoid implementing a Finalizer. Never define a **Finalize()** method as a finalizer. Instead use the C\# destructor syntax. 

    **Example**

    ```csharp
    // Good 
    ~MyClass {…} 

    // Bad 
    void Finalize(0) {…}
    ```

### Object Model & API Design

1. Always prefer aggregation over inheritance.

2. Avoid “Premature Generalization”. Create abstractions only when the intent
    is understood.

3. Do the simplest thing that works, then refactor when necessary.

4. Always make object-behavior transparent to API consumers.

5. Avoid unexpected side-affects when properties, methods, and constructors are
    invoked.

6. Always separate presentation layer from business logic.

7. Always prefer interfaces over abstract classes.

8. Try to include the design-pattern names such as “Bridge”, “Adapter”, or
    “Factory” as a suffix to class names where appropriate.

9. Only make members **virtual** if they are designed and tested for
    extensibility.

10. Refactor often!
