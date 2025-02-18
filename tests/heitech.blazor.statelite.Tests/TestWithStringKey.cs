using FluentAssertions;

namespace heitech.blazor.statelite.Tests;

public sealed class TestWithStringKey
{
    [Fact]
    public void WithStringKey()
    {
        // Arrange
        var sut = StateLiteFactory.Get<string>("test");

        // Act
        sut.Insert(new MyRecord("abc", "name"));
        sut.Insert(new MyRecord("123", "name2"));

        // Assert
        var all = sut.GetAll<MyRecord>();
        all.Should().HaveCount(2);
        
        var abc = sut.GetById<MyRecord>("abc");
        abc.Name.Should().Be("name");
        
        var one = sut.Query<MyRecord>(x => x.Id == "123").Single();
        one.Name.Should().Be("name2");
    }

    private sealed record MyRecord(string Id, string Name) : IHasId<string>;
}