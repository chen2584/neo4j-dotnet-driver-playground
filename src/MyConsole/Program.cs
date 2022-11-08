// See https://aka.ms/new-console-template for more information
using MyConsole;

using var movieGraphRepository = new MovieGraphRepository("bolt://localhost:7687", "neo4j", "some-very-secret-password");
var movie1 = movieGraphRepository.GetMovie("Forrest Gump");
Console.WriteLine($"Title: {movie1?.Title}, Released: {movie1?.Released}");

var movie2 = movieGraphRepository.GetMovie("Some Not Exist Title");
Console.WriteLine($"Title: {movie2?.Title}, Released: {movie2?.Released}");

var movieActors1 = movieGraphRepository.GetActors("Forrest Gump");
Console.WriteLine("-- Movie Actors 1 --");
foreach (var movieActor in movieActors1)
{
    Console.WriteLine($"Name: {movieActor.Name}, Roles: {String.Join(", ", movieActor.Roles)}");
}