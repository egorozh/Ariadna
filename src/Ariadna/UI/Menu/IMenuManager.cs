using System.Collections.Generic;

namespace Ariadna
{
    public interface IMenuManager
    {
        /// <summary>
        /// Заблокировать все корневые вкладки меню
        /// </summary>
        /// <param name="rootMenuHeaders">Исключения</param>
        void BlockAllRootMenuItems(string[] rootMenuHeaders = null);

        /// <summary>
        /// Разблокировать все корневые вкладки меню
        /// </summary>
        /// <param name="rootMenuHeaders">Исключения</param>
        void UnBlockAllRootMenuItems(string[] rootMenuHeaders = null);

        void Clear();

        void InitElements(List<UiMenuItem> uiMenuItems, List<UiIcon> uiFullIcons,
            List<UiKeyBinding> jsonConfigHotKeys,
            IReadOnlyList<IFeature> akimFeatures, AriadnaApp ariadnaApp);
    }
}       