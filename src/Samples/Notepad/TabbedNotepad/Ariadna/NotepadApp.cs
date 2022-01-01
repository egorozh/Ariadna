using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Ariadna;
using Ariadna.Core;

namespace TabbedNotepad;

public class NotepadApp : BaseViewModel, INotepadApp
{
    #region Private Fields

    private readonly ObservableCollection<IDocumentModel> _projects = new();

    #endregion

    public AriadnaApp AriadnaApp { get; }
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
        return Application.Current.FindResource("AkimDocumentHeaderTemplate") as DataTemplate;
    }

    public void Init(IEnumerable tools)
    {
        Tools = new ObservableCollection<IToolViewModel>((IEnumerable<IToolViewModel>)tools);
    }

    public ICollection<IToolViewModel> Tools { get; private set; }
    public ICollection<IDocumentModel> Projects => _projects;
    public IDocumentModel? CurrentProject { get; set; }
}