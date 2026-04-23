namespace PokeCraft.Cms.Tools;

public class ToolsSerializer : ISerializer
{
  private static ToolsSerializer? _instance = null;
  public static ISerializer Instance
  {
    get
    {
      _instance ??= new();
      return _instance;
    }
  }

  private readonly JsonSerializerOptions _serializerOptions = new();

  private ToolsSerializer()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    _serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    _serializerOptions.WriteIndented = true;
  }

  public T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _serializerOptions);
  public string Serialize<T>(T value) => JsonSerializer.Serialize(value, _serializerOptions);
}
