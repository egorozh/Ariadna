using Ariadna.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace Ariadna;

public class UiOptions : BaseViewModel, IUiOptions
{
    #region Private Fields

    private readonly IConfiguration _configuration;
    private readonly AriadnaApp _ariadnaApp;
    private readonly IStorage _storage;

    #endregion

    #region Public Properties

    public ThemeOptions Theme { get; set; } = new();

    public bool IsShowRibbon { get; set; }

    #endregion

    #region Constructor

    public UiOptions(IConfiguration configuration, AriadnaApp ariadnaApp,
        IStorage storage)
    {
        _configuration = configuration;
        _ariadnaApp = ariadnaApp;
        _storage = storage;

        Theme = _configuration.GetSection("Theme").Get<ThemeOptions>();
        IsShowRibbon = Convert.ToBoolean(configuration["IsShowRibbon"]);

        Theme.PropertyChanged += Theme_PropertyChanged;
        PropertyChanged += Theme_PropertyChanged;
    }

    #endregion

    #region Private Methods

    private void Theme_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        AddOrUpdateAppSetting("IsShowRibbon", IsShowRibbon);
        AddOrUpdateAppSetting("Theme:Theme", Theme.Theme);
        AddOrUpdateAppSetting("Theme:Accent", Theme.Accent);
    }

    public void AddOrUpdateAppSetting<T>(string key, T value)
    {
        try
        {
            var filePath = _storage.AppSettingsPath; 
            string json = File.ReadAllText(filePath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            SetValueRecursively(key, jsonObj, value);

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(filePath, output);
        }
        catch (Exception)
        {
            Console.WriteLine("Error writing app settings");
        }
    }

    private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
    {
        // split the string at the first ':' character
        var remainingSections = sectionPathKey.Split(":", 2);

        var currentSection = remainingSections[0];
        if (remainingSections.Length > 1)
        {
            // continue with the procress, moving down the tree
            var nextSection = remainingSections[1];
            SetValueRecursively(nextSection, jsonObj[currentSection], value);
        }
        else
        {
            // we've got to the end of the tree, set the value
            jsonObj[currentSection] = value;
        }
    }

    #endregion
}