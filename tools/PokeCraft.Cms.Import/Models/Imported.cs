using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Import.Models;

internal record Imported<T>(T Entity, ContentPayload Content);
