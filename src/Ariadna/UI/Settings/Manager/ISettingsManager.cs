using System.Collections.Generic;

namespace Ariadna
{
    public interface ISettingsManager
    {
        void Show();
        void Close();

        void Clear();
        void InitElements(List<UiSettingsItem> uiSettingsItems, IEnumerable<ISettings> settings);
    }
}