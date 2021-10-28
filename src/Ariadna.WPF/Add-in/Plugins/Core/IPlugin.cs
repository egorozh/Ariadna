using Autofac;

namespace Ariadna;

public interface IPlugin
{
    string Name { get; }
    
    string? GetResourceUri();

    void Init(ContainerBuilder builder);

    void Init<TParameter>(ContainerBuilder builder, TParameter parameter) where TParameter : class;
}