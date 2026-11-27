using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tellurian.Utilities.Tests;

internal sealed record ModuleData(int Id, string Name);

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(ModuleData))]
internal sealed partial class TestJsonContext : JsonSerializerContext;
