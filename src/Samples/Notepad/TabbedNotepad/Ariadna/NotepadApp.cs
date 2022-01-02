using Ariadna;
using Ariadna.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace TabbedNotepad;

public class NotepadApp : BaseViewModel, INotepadApp
{
    #region Private Fields

    private readonly ObservableCollection<IDocumentViewModel> _projects = new();

    #endregion

    public AriadnaApp AriadnaApp { get; }
    public ICollection<IToolViewModel> Tools { get; private set; }
    public ICollection<IDocumentViewModel> Projects => _projects;
    public IDocumentViewModel? CurrentProject { get; set; }
    
    public event EventHandler? CurrentProjectChanged;
    public event EventHandler<NotifyCollectionChangedEventArgs>? ProjectsChanged;

    public NotepadApp(AriadnaApp ariadnaApp)
    {
        AriadnaApp = ariadnaApp;
        ExPropertyChanged += AkimEditorApp_ExPropertyChanged;
    }

    private void AkimEditorApp_ExPropertyChanged(object? sender, ExPropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CurrentProject))
            CurrentProjectChanged?.Invoke(this, EventArgs.Empty);
    }

    public DataTemplate? GetDocumentHeaderTemplate()
    {
        var t= Application.Current.FindResource("DocumentHeaderTemplate") as DataTemplate;
        return t;
    }

    public void Init(IEnumerable tools)
    {
        Tools = new ObservableCollection<IToolViewModel>((IEnumerable<IToolViewModel>) tools);
    }
}