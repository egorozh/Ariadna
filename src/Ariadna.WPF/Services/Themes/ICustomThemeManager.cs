namespace Ariadna;

public interface ICustomThemeManager
{
    void SetTheme(IThemeAndAccentManager themeManager);
    void Init();
}