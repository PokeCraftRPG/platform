# PokeCraft.Cms.PostgreSQL

Provides the PostgreSQL-specific infrastructure implementation for the project’s persistence, migrations, and data access concerns.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the solution directory.

### Create a migration

To create a new migration, execute the following command. Do not forget to provide a migration name!

```sh
dotnet ef migrations add <YOUR_MIGRATION_NAME> --context PokemonContext --project src/PokeCraft.Cms.PostgreSQL --startup-project src/PokeCraft.Cms
```

### Remove a migration

To remove the latest unapplied migration, execute the following command.

```sh
dotnet ef migrations remove --context PokemonContext --project src/PokeCraft.Cms.PostgreSQL --startup-project src/PokeCraft.Cms
```

### Generate a script

To generate a script, execute the following command. Do not forget to provide a source migration name!

```sh
dotnet ef migrations script <SOURCE_MIGRATION> --context PokemonContext --project src/PokeCraft.Cms.PostgreSQL --startup-project src/PokeCraft.Cms
```
