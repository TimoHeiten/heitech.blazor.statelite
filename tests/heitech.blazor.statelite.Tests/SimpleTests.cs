using FluentAssertions;

namespace heitech.blazor.statelite.Tests;

public sealed class SimpleTests
{
    private static IStateLite<Guid> CreateSut(string name = "test")
        => StateLiteFactory.Get<Guid>(name);

    [Fact]
    public void Store_And_Retrieve()
    {
        // Arrange
        using var sut = CreateSut(nameof(Store_And_Retrieve));
        var obj = CreateAndInsert(sut);

        // Act
        var result = sut.GetById<ObjectOne>(obj.Id);

        // Assert
        result.Should().BeEquivalentTo(obj);
    }

    [Fact]
    public void Store_And_Delete()
    {
        // Arrange
        using var sut = CreateSut(nameof(Store_And_Delete));
        var obj = CreateAndInsert(sut);

        // Act
        sut.Delete(obj);
        var result = sut.GetById<ObjectOne>(obj.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Store_Multiple_and_GetAll()
    { 
        // Arrange
        using var sut = CreateSut(nameof(Store_Multiple_and_GetAll));
        CreateAndInsert(sut);
        CreateAndInsert(sut);
        CreateAndInsert(sut);

        // Act
        var all = sut.GetAll<ObjectOne>();

        // Assert
        all.Should().HaveCount(3);
    }

    [Fact]
    public void FindByComplexFilter()
    {
        // Arrange 
        using var sut = CreateSut(nameof(FindByComplexFilter));
        var objectOnes = Enumerable.Range(0, 5).Select(CreateOne).ToList();
        var coll = new ComplexObject(Guid.NewGuid(), objectOnes);
        sut.Insert(coll);

        // Act
        var result = sut.Query<ComplexObject>(x => x.ObjectOnes.Any());

        // Assert
        result.Single().Should().BeEquivalentTo(coll);
    }

    [Fact]
    public void Purge_returns_empty_Collection()
    {
        // Arrange
        using var sut = CreateSut(nameof(Purge_returns_empty_Collection));
        CreateAndInsert(sut);
        CreateAndInsert(sut);
        CreateAndInsert(sut);

        // Act
        sut.Purge();

        // Assert
        sut.GetAll<ObjectOne>().Should().BeEmpty();
    }

    private ObjectOne CreateAndInsert(IStateLite<Guid> lite, ObjectOne? one = null)
    {
        var oneToInsert = one ?? CreateOne();
        lite.Insert(oneToInsert);
        return oneToInsert;
    }

    private static ObjectOne CreateOne(int i = 0)
        => new(Guid.NewGuid(), $"key-{i}", $"value- {i}");

    private sealed record ObjectOne(Guid Id, string Key, string Value) : IHasId<Guid>
    {
        public ObjectOne() : this(Guid.Empty, string.Empty, string.Empty)
        { }
    }

    private sealed record ComplexObject(Guid Id, List<ObjectOne> ObjectOnes) : IHasId<Guid>
    {
        public ComplexObject() : this(Guid.Empty, new List<ObjectOne>(0))
        {
            
        }
    }
}