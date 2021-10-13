using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Ariadna.QuickActionsBar;

public partial class ToolBarLayout
{
    #region Dependency Properties

    public static readonly DependencyProperty TopToolBarProperty = DependencyProperty.Register(
        nameof(TopToolBar), typeof(ObservableCollection<ObservableCollection<Control>>), typeof(ToolBarLayout),
        new PropertyMetadata(default(ObservableCollection<ObservableCollection<Control>>)));

    public static readonly DependencyProperty LeftToolBarProperty = DependencyProperty.Register(
        nameof(LeftToolBar), typeof(ObservableCollection<ObservableCollection<Control>>), typeof(ToolBarLayout),
        new PropertyMetadata(default(ObservableCollection<ObservableCollection<Control>>)));

    public static readonly DependencyProperty RightToolBarProperty = DependencyProperty.Register(
        nameof(RightToolBar), typeof(ObservableCollection<ObservableCollection<Control>>), typeof(ToolBarLayout),
        new PropertyMetadata(default(ObservableCollection<ObservableCollection<Control>>)));

    public static readonly DependencyProperty IsShowLeftProperty = DependencyProperty.Register(
        "IsShowLeft", typeof(bool), typeof(ToolBarLayout), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty IsShowTopProperty = DependencyProperty.Register(
        "IsShowTop", typeof(bool), typeof(ToolBarLayout), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty IsShowRightProperty = DependencyProperty.Register(
        "IsShowRight", typeof(bool), typeof(ToolBarLayout), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty QuickActionsContextMenuProperty = DependencyProperty.Register(
        "QuickActionsContextMenu", typeof(ContextMenu), typeof(ToolBarLayout),
        new PropertyMetadata(default(ContextMenu)));

    public ContextMenu QuickActionsContextMenu
    {
        get => (ContextMenu) GetValue(QuickActionsContextMenuProperty);
        set => SetValue(QuickActionsContextMenuProperty, value);
    }

    #endregion

    #region Public Properties

    public ObservableCollection<ObservableCollection<Control>> TopToolBar
    {
        get => (ObservableCollection<ObservableCollection<Control>>) GetValue(TopToolBarProperty);
        set => SetValue(TopToolBarProperty, value);
    }

    public ObservableCollection<ObservableCollection<Control>> LeftToolBar
    {
        get => (ObservableCollection<ObservableCollection<Control>>) GetValue(LeftToolBarProperty);
        set => SetValue(LeftToolBarProperty, value);
    }

    public ObservableCollection<ObservableCollection<Control>> RightToolBar
    {
        get => (ObservableCollection<ObservableCollection<Control>>) GetValue(RightToolBarProperty);
        set => SetValue(RightToolBarProperty, value);
    }

    public bool IsShowLeft
    {
        get => (bool) GetValue(IsShowLeftProperty);
        set => SetValue(IsShowLeftProperty, value);
    }

    public bool IsShowTop
    {
        get => (bool) GetValue(IsShowTopProperty);
        set => SetValue(IsShowTopProperty, value);
    }

    public bool IsShowRight
    {
        get => (bool) GetValue(IsShowRightProperty);
        set => SetValue(IsShowRightProperty, value);
    }

    #endregion

    #region Constructor

    public ToolBarLayout()
    {
        InitializeComponent();
    }

    #endregion
}