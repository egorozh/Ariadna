using Serilog;
using System.IO;
using System.Text.Json;

namespace Ariadna;

/// <summary>
/// Структура Json документа, в котором хранится информация о расположении кнопок в интерфейсе
/// </summary>
public class JsonInterface
{
    public List<UiMenuItem> Menu { get; set; } = new();

    public List<UiTabRibbon> Ribbon { get; set; } = new();

    public UiQuickActions QuickActions { get; set; } = new();

    public List<UiSettingsItem> Settings { get; set; } = new();

    public List<UiIcon> Icons { get; set; } = new();

    public List<UiHelpVideo> HelpVideos { get; set; } = new();

    public List<UiKeyBinding> HotKeys { get; set; } = new();

    public static void SaveJsonScheme(JsonInterface config, string path, ILogger logger)
    {
        try
        {
            var settings = new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };

            var jsonStroke = JsonSerializer.Serialize(config, settings);

            File.WriteAllText(path, jsonStroke);
        }
        catch (Exception e)
        {
            logger.Error(e.Message);
        }
    }
}