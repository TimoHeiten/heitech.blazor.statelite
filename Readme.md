
## In Memory state with abstracted LiteDb (using memory stream)
Primarly built to be used with Blazor as a sort of central State management, without using the complex flow of Flux or others.
And without reinventing the stuff for each application.

### Get started

#### install
```bash
dotnet add package heitech.blazor.statelite
```

#### get hold of a store
Either use the Factory to register a named store
Or use the Configuration extension to register a singleton with the service collection
```csharp
// via factory - thread safe and with in Memory lookup
var storeOne = StateLiteFactory.Get("one");
var storeTwo = StateLiteFactory.Get("two");

// with Services
builder.Services.AddStateLite();

// and then via DI
public sealed class C1
{
	//...
    public C1(IStateLite stateLite)
	{
		// ...
	}
}

```
#### interface examples (see also test class for basic examples)

```csharp
var store = StateLiteFactory.Get("myStore");

var id = Guid.NewGuid();
store.Insert(new MyStroreType(id, "Lorem ipsum dolor");

var result = store.GetById(id);
System.Diagnostics.Debug.Assert(result != null);

store.Delete(result);

// etc.
private sealed record MyStoreType(Guid Id, string SomeData) : IHasId;

```


### LiteDb why?
- simple in memory
- simple c# interface
- lightweight

### Next up
- Slite in memory db (with native dlls)