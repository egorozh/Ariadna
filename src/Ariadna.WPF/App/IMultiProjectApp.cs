using System.Collections.Specialized;
using System.Windows;

namespace Ariadna;

public interface IMultiProjectApp<TDoc, TTool> : IMultiProjectApp
    where TDoc : class, IDocumentViewModel
    where TTool : IToolViewModel
{
    ICollection<TTool> Tools { get; }

    ICollection<TDoc> Projects { get; }
    TDoc? CurrentProject { get; set; }
}

public interface IMultiProjectApp
{
    AriadnaApp AriadnaApp { get; }


    event EventHandler CurrentProjectChanged;
    event EventHandler<NotifyCollectionChangedEventArgs> ProjectsChanged;

    DataTemplate? GetDocumentHeaderTemplate();

    void Init(global::System.Collections.IEnumerable tools);
}