using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace heitech.blazor.stateXt.tests;

public sealed class IntegrationTestWithDI
{
    [Fact]
    public void Register_And_Resolve_Works_Returns_Singleton_For_A_Service_Indepedent_Of_Scope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.RegisterState<StateService, TestStateObject?>();
        var serviceProvider = services.BuildServiceProvider();
        
        // Act
        var producer = serviceProvider.GetRequiredService<IProduceState<TestStateObject>>();
        var consumer = serviceProvider.GetRequiredService<IConsumeState<TestStateObject>>();
        
        // Assert
        producer.Should().BeSameAs(consumer);
        var scope = serviceProvider.CreateScope();
        var state = scope.ServiceProvider.GetRequiredService<IProduceState<TestStateObject>>();
        state.Should().BeSameAs(producer);
    }
    
    private sealed record TestStateObject(int Id, string Name);
    private sealed class StateService : LatestStateNotificationService<TestStateObject?>;
}