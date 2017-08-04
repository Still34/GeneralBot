## TypeReaders ##

All `TypeReader`s declared here must have a `public static Type` property named `Type` for the `TypeReader` to be added automatically.

i.e.
```cs
public static Type Type { get; } = typeof(MyClass);
```