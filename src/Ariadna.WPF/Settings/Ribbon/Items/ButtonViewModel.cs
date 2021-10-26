using System.Windows;

namespace Ariadna.Settings;

internal class ButtonViewModel : RibbonItemViewModel
{
    #region Public Properties

    public RibbonItemSize Size { get; set; }

    public string[] Sizes { get; } =
    {
        "Large",
        "Middle",
        "Small"
    };

    public string SelectedSize { get; set; }

    public FrameworkElement Icon { get; }

    public FrameworkElement IconLarge { get; }
        
    #endregion

    #region Constructor

    public ButtonViewModel(UiRibbonItem ribbonItem, IFeature feature) : base(ribbonItem, feature)
    {
        Size = ribbonItem.Size; 

        if (Feature is ICommandFeature commandFeature)
        {
            Icon = commandFeature.GetDefaultIcon();
            IconLarge = commandFeature.GetDefaultIcon();
        }

        SelectedSize = Sizes[(int) Size];
            
        PropertyChanged += ButtonViewModel_PropertyChanged;
    }

    #endregion

    #region Private Methods

    private void ButtonViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedSize))
        {
            Size = (RibbonItemSize) Array.IndexOf(Sizes, SelectedSize);
        }
    }

    #endregion
}