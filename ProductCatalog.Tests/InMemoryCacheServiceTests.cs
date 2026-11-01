using ProductCatalog.Api.Services;

namespace ProductCatalog.Tests;

public class InMemoryCacheServiceTests
{
    [Fact]
    public void Get_ReturnsValue_AfterSet()
    {
        var cache = new InMemoryCacheService();
        cache.Set("k", "v", TimeSpan.FromMinutes(1));
        Assert.Equal("v", cache.Get<string>("k"));
    }

    [Fact]
    public void Get_ReturnsNull_WhenExpired()
    {
        var cache = new InMemoryCacheService();
        cache.Set("k", "v", TimeSpan.FromMilliseconds(-1));
        Assert.Null(cache.Get<string>("k"));
    }

    [Fact]
    public void Remove_DeletesEntry()
    {
        var cache = new InMemoryCacheService();
        cache.Set("k", "v", TimeSpan.FromMinutes(1));
        cache.Remove("k");
        Assert.Null(cache.Get<string>("k"));
    }
}