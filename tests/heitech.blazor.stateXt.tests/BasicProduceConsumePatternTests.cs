using FluentAssertions;

namespace heitech.blazor.stateXt.tests;

public sealed partial class BasicProduceConsumePatternTests
{
    [Fact]
    public async Task Add_Consumer_And_Produce_Value_Updates()
    {
        // Arrange        
        var (producer, consumer) = ArrangeSut();
        TestStateObject propInComponent = null!;
        await consumer.SubscribeAsync(newState => propInComponent = newState, _fakeComponentAsReceiver);
        
        // Act
        await producer.SetValue(new TestStateObject(42, "test state"));

        // Assert
        AssertTestState(propInComponent);
    }
    
    [Fact]
    public async Task Add_Consumer_And_Unsub_Before_Produce_No_Value_Updates()
    {
        // Arrange        
        var (producer, consumer) = ArrangeSut();

        TestStateObject propInComponent = null!;
        await consumer.SubscribeAsync(newState => propInComponent = newState, _fakeComponentAsReceiver);
        
        // Act
        consumer.Unsubscribe(_fakeComponentAsReceiver);
        await producer.SetValue(new TestStateObject(42, "test state"));

        // Assert
        propInComponent.Should().BeNull();
    }
    
    [Fact]
    public async Task Consecutive_State_Changes_On_Consumer_Get_Updated_For_Multiple_Consumers()
    {
        // Arrange        
        var (producer, consumer) = ArrangeSut();

        TestStateObject propInComponent = null!;
        TestStateObject propInComponent2 = null!;
        await consumer.SubscribeAsync(newState => propInComponent = newState, _fakeComponentAsReceiver);
        await consumer.SubscribeAsync(newState => propInComponent2 = newState, _fakeComponentAsReceiver);
        
        // Act
        await producer.SetValue(new TestStateObject(42, "test state"));

        // Assert
        AssertTestState(propInComponent);
        AssertTestState(propInComponent2);
        
        
        // second produce
        await producer.SetValue(new TestStateObject(43, "test state 2"));
        AssertTestState(propInComponent, 43, "test state 2");
        AssertTestState(propInComponent2, 43, "test state 2");

    }
}