using FluentAssertions;

namespace heitech.blazor.stateXt.tests;

public sealed partial class BasicProduceConsumePatternTests
{
    private readonly object _fakeComponentAsReceiver = new();
    private readonly object _secondFakeComponentAsReceiver = new();
    
    private static (IProduceState<TestStateObject> producer, IConsumeState<TestStateObject> consumer) ArrangeSut()
    {
        var sut = new StateService();
        return (sut, sut);
    }

    private static void AssertTestState(TestStateObject simulatedComponentProperty, int id = 42, string name = "test state")
    {
        simulatedComponentProperty.Should().BeEquivalentTo(new
        {
            Id = id,
            Name = name
        });
    }
    
    
    private sealed record TestStateObject(int Id, string Name);
    private sealed class StateService : LatestStateNotificationService<TestStateObject>;
}