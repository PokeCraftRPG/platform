namespace PokeCraft.Cms.Seeding;

internal record Failure<T>(T Value, Exception Exception);
