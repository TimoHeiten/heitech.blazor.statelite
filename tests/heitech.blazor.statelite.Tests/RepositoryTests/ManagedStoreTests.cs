using FluentAssertions;
using heitech.blazor.statelite.repositories;

namespace heitech.blazor.statelite.Tests.RepositoryTests;

public sealed partial class ManagedStoreTests
{
    [Fact]
    public async Task UpsertItems_Should_Notify_All_Subscribers()
    {
        // Arrange
        var initialEntities = ArrangeEntities();
        var store = new repositories.MangedState<int>(StoreName);
        var sub1 = new Sub();
        var sub2 = new Sub();
        store.Subscribe(sub1);
        store.Subscribe(sub2);

        // Act
        await store.UpsertItems(() => Task.FromResult(initialEntities));

        // Assert 
        sub1.LatestChanges.Should().HaveCount(3);
        sub2.LatestChanges.Should().HaveCount(3);

        store.Store.GetAll<Entities>().Should().HaveCount(3);
        store.Store.Query<Entities>(x => x.Inners.Length == 2).Should().HaveCount(1);
        store.Store.GetById<Entities>(1).Should().NotBeNull();
    }

    [Fact]
    public void Add_And_Remove_Subscribers_Works()
    {
        // Arrange
        var ms = EnterState.Go<int>("sub_store");
        var sub = new Sub();
        var entities = ArrangeEntities();
        ms.Subscribe(sub);
        ms.UpsertItems(() => Task.FromResult(entities));
        ms.Unsubscribe(sub);

        // Act
        ms.DeleteItemsByKey<Entities>(() => Task.CompletedTask, entities.Take(1).Select(x => x.Id));

        // Assert
        // if no unsub it would be 2 since thats the last notification after delete
        sub.LatestChanges.Should().HaveCount(3);
    }
}