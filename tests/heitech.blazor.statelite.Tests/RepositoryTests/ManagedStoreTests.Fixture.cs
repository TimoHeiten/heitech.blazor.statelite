using heitech.blazor.statelite.repositories;

namespace heitech.blazor.statelite.Tests.RepositoryTests;

public sealed partial class ManagedStoreTests
{
    private const string StoreName = "managed-store-test";

    public static IEnumerable<Entities> ArrangeEntities()
    {
        yield return new Entities(1, "Test", new Inner("1", "Test"));
        yield return new Entities(2, "Test", new Inner("2a", "Test"), new Inner("2b", "Test"));
        yield return new Entities(3, "Test", new Inner("3", "Test"));
    }

    public sealed class Entities : IHasId<int>
    {
        public Entities(int id, string name, params Inner[] inner)
        {
            Id = id;
            Name = name;
            Inners = inner;
        }
        // for serialization
        public Entities()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public Inner[] Inners { get; set; } = default!;
    }

    public sealed record Inner(string Identifier, string Value);
    
    private sealed class Sub : ISubscriber<int>
    {
        public void OnChanges<T>(IEnumerable<T> items) where T : IHasId<int>, new()
        {
            LatestChanges = (items as IEnumerable<Entities>)!;
        }

        public IEnumerable<Entities> LatestChanges { get; private set; } = null!;
    }
}