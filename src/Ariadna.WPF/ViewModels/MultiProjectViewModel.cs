using Ariadna.Core;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using System.IO;

namespace Ariadna;

internal class MultiProjectViewModel : BaseViewModel
{
    private readonly IStorage _storage;

    #region Public Properties

    public IMultiProjectApp MultiProjectApp { get; }
    public DockingManager? DockingManager { get; private set; }

    #endregion

    #region Constructor

    public MultiProjectViewModel(IMultiProjectApp multiProjectApp,
        IStorage storage)
    {
        _storage = storage;
        MultiProjectApp = multiProjectApp;
    }

    #endregion

    #region Public Methods

    public void Init(DockingManager dockingManager)
    {
        DockingManager = dockingManager;

        var template = MultiProjectApp.GetDocumentHeaderTemplate();

        if (template != null)
            DockingManager.DocumentHeaderTemplate = template;
    }

    public void Load()
    {
        var serializer = new XmlLayoutSerializer(DockingManager);
        serializer.LayoutSerializationCallback += Serializer_LayoutSerializationCallback;

        if (File.Exists(GetAvalonConfigPath()))
            serializer.Deserialize(GetAvalonConfigPath());
    }

    public void Unload()
    {
        var serializer = new XmlLayoutSerializer(DockingManager);

        serializer.Serialize(GetAvalonConfigPath());
    }

    #endregion

    #region Private Methods

    private static void Serializer_LayoutSerializationCallback(object? sender,
        LayoutSerializationCallbackEventArgs e)
    {
        e.Content = e.Content;

        if (e.Content == null)
            e.Cancel = true;
    }

    private string GetAvalonConfigPath() => _storage.AvalonConfigPath;

    #endregion
}