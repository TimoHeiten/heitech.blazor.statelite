using FluentAssertions;

namespace heitech.blazor.statelite.Tests.RepositoryTests;

public sealed partial class ManagedStoreTests
{
    [Fact]
    public async Task UpsertItems_Should_Notify_All_Subscribers()
    {
        // Arrange
        var initialEntities = ArrangeEntities();
        var store = new repositories.MangedStore<int>(StoreName);
        var sub1 = new Sub();
        var sub2 = new Sub();
        store.Subscribe(sub1);
        store.Subscribe(sub2);

        // Act
        await store.UpsertItems(() => Task.FromResult(initialEntities));

        // Assert 
        sub1.LastChanges.Should().HaveCount(3);
        sub2.LastChanges.Should().HaveCount(3);

        store.Store.GetAll<Entities>().Should().HaveCount(3);
        store.Store.Query<Entities>(x => x.Inners.Length == 2).Should().HaveCount(1);
    }
}