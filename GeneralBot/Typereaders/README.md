## TypeReaders ##

All `TypeReader`s declared here must have a `public static Type[]` array property named `Types` for the `TypeReader` to be added automatically.

i.e.
```cs
public static Type[] Types { get; } = {typeof(MyClass)};
```