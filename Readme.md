# two flavors of state management (I happen to use frequently in blazor wasm)
Simple, effective and easy to work with.

2 different use cases and patterns of use. See below.

## 1. (Behavior)Subject State Service 
A service that can be registered and allows for independent Producer and Consumer services that can be Injected to your components.
You could also use it in other cases, but it was developed with blazor WASM in mind.

Producer sets a value; a Consumer (might be same component or many different components listening to the broadcast) receives the value and reacts via a registered callback.
Producer/Consumer should be injected into your components to decouple components and depend on state abstractions instead of bindings / InputParameters etc.

You will need a combination of State objects and StateServices. See example below.

install
```bash 
dotnet add package heitech.blazor.stateXt
# optional for oob configure methods for MS DI:
dotnet add package heitech.blazor.stateXt.extensions
```

Example usage (see also tests/heitech.blazor.stateXt.tests)
State.cs
```csharp
// the state object you want to move between components (could also just be an empty record for message style patterns)
private sealed record StateObject(int Id, string Name);
// the service making use of the ABC - you will need one for each type of state you want to use in the app
// (you could also simply use one and do a type switch, works but not recommended from a design perspective imo)
private sealed class StateService : LatestStateNotificationService<StateObject>
{
    // initial (optional) state; all subscribers will receive the latest state. Which might be null initially
    // so this is recommended, if feasible.
    public StateService() : base(new StateObject(42, "MyStateManagement"){ }
}

```

Program.cs
```csharp
// then register (either by yourself or use the ready made configure method from the package heitech.blazor.stateXt.extensions
// but make sure the services are registered as SINGLETON or else there might be weird errors
services.RegisterState<StateService, StateObject?>();
// injects the StateService and the interfaces IProduceState<T> as well as IConsumeState<T> which should be preferred. (Abstraction, Componenttests etc.)
```

ProducingComponent.razor.cs
```csharp
// producing component 1
<button onClick="@(() => NextSampleState())">Produce rnd state</button>

@code {
    [Inject]
    private IProduceState<StateObject> _producer { get; set; } = null!;
    
    private async Task NextSampleState() 
    {
        var rnd = new Random();
        await _producer.SetValue(new StateObject(rnd.Next(), "state name");
    }
}
```

ConsumeComponent.razor.cs
```csharp
// ... and consuming component 2
<h1>@_state?.Name</h1>

@code {
    
    private StateObject? _state = null!;
    
    [Inject]
    private IConsumeState<StateObject> _consumer { get; set; } = null!;
    
    protected async Task OnInitializedAsync()
    {
        // actual subscribe, the action callback will be notified with latest state and on every state change.
        await _consumer.SubscribeAsync(newState => _state = newState, receiver: this);
        await base.OnInitializedAsync();
    }
    
    // important! - to avoid mem leaks
    public void Dispose()
    {
        _consumer?.Unsubscribe(this);    
    }
}
```


## 2. In Memory state with abstracted LiteDb (using memory stream)
Primarily built to be used with Blazor as a sort of central State management, without using the complex flow of Flux or others.
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

