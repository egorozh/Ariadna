using Autofac;

namespace Ariadna;

/// <summary>
/// Интерфейс плагина AKIM
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Название плагина
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Получение названия основного словаря ресурсов модуля
    /// </summary>
    /// <returns></returns>
    string? GetResourceUri();

    void Init(ContainerBuilder builder);

    void Init<TParameter>(ContainerBuilder builder, TParameter parameter) where TParameter : class;
}