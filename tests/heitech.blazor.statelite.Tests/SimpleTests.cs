using FluentAssertions;

namespace heitech.blazor.statelite.Tests;

public sealed class SimpleTests
{
    private readonly IStateLite _state;
    public SimpleTests()
    {
        _state = StateLiteFactory.Get("test");
    }

    [Fact]
    public void Store_And_Retrieve()
    {
        // Arrange
        var obj = CreateAndInsert();

        // Act
        var result = _state.GetById<ObjectOne>(obj.Id);

        // Assert
        result.Should().BeEquivalentTo(obj);
    }
    
    [Fact]
    public void Store_And_Delete()
    {
        // Arrange
        var obj = CreateAndInsert();

        // Act
        _state.Delete(obj);
        var result = _state.GetById<ObjectOne>(obj.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Store_Multiple_and_GetAll()
    { 
        // Arrange
        CreateAndInsert();
        CreateAndInsert();
        CreateAndInsert();

        // Act
        var all = _state.GetAll<ObjectOne>();

        // Assert
        all.Should().HaveCount(3);
    }

    [Fact]
    public void FindByComplexFilter()
    {
        // Arrange 
        var objectOnes = Enumerable.Range(0, 5).Select(CreateOne).ToList();
        var coll = new ComplexObject(Guid.NewGuid(), objectOnes);
        _state.Insert(coll);

        // Act
        var result = _state.Query<ComplexObject>(x => x.ObjectOnes.Any());

        // Assert
        result.Single().Should().BeEquivalentTo(coll);
    }

    private ObjectOne CreateAndInsert(ObjectOne? one = null)
    {
        var oneToInsert = one ?? CreateOne();
        _state.Insert(oneToInsert);
        return oneToInsert;
    }

    private static ObjectOne CreateOne(int i = 0)
        => new(Guid.NewGuid(), $"key-{i}", $"value- {i}");

    private sealed record ObjectOne(Guid Id, string Key, string Value) : IHasId;
    private sealed record ComplexObject(Guid Id, List<ObjectOne> ObjectOnes) : IHasId;
}