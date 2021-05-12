using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Serilog;

namespace Ariadna
{
    /// <summary>
    /// Структура Json документа, в котором хранится информация о расположении кнопок в интерфейсе
    /// </summary>
    public class JsonInterface
    {
        public List<UiMenuItem> Menu { get; set; }

        public List<UiTabRibbon> Ribbon { get; set; }

        public UiQuickActions QuickActions { get; set; }

        public List<UiSettingsItem> Settings { get; set; }

        public List<UiIcon> Icons { get; set; } = new List<UiIcon>();

        public List<UiHelpVideo> HelpVideos { get; set; } = new List<UiHelpVideo>();

        public List<UiKeyBinding> HotKeys { get; set; } = new List<UiKeyBinding>();

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
}