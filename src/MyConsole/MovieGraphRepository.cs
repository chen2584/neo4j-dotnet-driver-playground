using System.Diagnostics.Metrics;
using System.Security;
using Neo4j.Driver;

namespace MyConsole;

public class MovieDto
{
    public string Title { get; set; } = null!;
    public int? Released { get; set; }
}

public class MovieActorDto
{
    public string Name { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = null!;
}

public class MovieGraphRepository : IDisposable
{
    private bool _disposed;
    readonly IDriver _driver;

    ~MovieGraphRepository() => Dispose(false);
    public MovieGraphRepository(string uri, string user, string password)
    {
        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _driver?.Dispose();
        }

        _disposed = true;
    }

    public MovieDto? GetMovie(string title)
    {
        using var session = _driver.Session();
        var movie = session.ExecuteRead(i =>
        {
            var data = i.Run("MATCH (m:Movie { title: $title }) RETURN m.title, m.released", new { title });
            var firstData = data.FirstOrDefault();
            return firstData != null ? new MovieDto
            {
                Title = firstData[0].As<string>(),
                Released = firstData[1].As<int?>()
            } : null;
        });

        return movie;
    }

    public IEnumerable<MovieActorDto> GetActors(string title)
    {
        using var session = _driver.Session();
        var movieActors = session.ExecuteRead(i =>
        {
            var data = i.Run("MATCH (a:Actor)-[r:ACTED_IN]->(m:Movie { title: $title }) RETURN a.name, r.roles", new { title });
            return data.Select(j => new MovieActorDto
            {
                Name = j[0].As<string>(),
                Roles = j[1].As<IEnumerable<string>?>() ?? Enumerable.Empty<string>()
            }).ToList(); // ToList() Is Required to fetch data for now or throw Neo4j.Driver.ResultConsumedException
        });

        return movieActors;
    }
}