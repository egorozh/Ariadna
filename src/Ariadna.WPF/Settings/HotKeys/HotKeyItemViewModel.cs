using System.ComponentModel;
using Ariadna.Core;
using MahApps.Metro.Controls;

namespace Ariadna.Settings;

internal class HotKeyItemViewModel : BaseViewModel
{
    private readonly HotKeysSettingsViewModel _mainVm;
   
    public HotKey Keys { get; set; }

    public string Header { get; set; }

    public ICommandFeature CommandFeature { get; }

    public HotKeyItemViewModel(ICommandFeature commandFeature,
        IInterfaceHelper interfaceHelper,
        List<UiKeyBinding> hotKeys,
        HotKeysSettingsViewModel mainVm)
    {
        _mainVm = mainVm;
        CommandFeature = commandFeature;

        Header = commandFeature.ToString();

        var kb = interfaceHelper.CreateKeyBinding(hotKeys, commandFeature);

        if (kb != null)
            Keys = new HotKey(kb.Key, kb.Modifiers);

        PropertyChanged += HotKeyItemViewModel_PropertyChanged;
    }

    private void HotKeyItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e is ExPropertyChangedEventArgs args)
        {
            if (e.PropertyName == nameof(Keys))
            {
                _mainVm.KeysChanged(this,(HotKey) args.OldValue, (HotKey)args.NewValue);
            }
        }
    }

    public string Error => string.Empty;

    public void SetKeys(HotKey hotKey)
    {
        PropertyChanged -= HotKeyItemViewModel_PropertyChanged;

        Keys = hotKey;

        PropertyChanged += HotKeyItemViewModel_PropertyChanged;
    }
}