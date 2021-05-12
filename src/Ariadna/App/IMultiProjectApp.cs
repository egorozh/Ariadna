using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace Ariadna
{
    public interface IMultiProjectApp<TDoc, TTool> : IMultiProjectApp
        where TDoc : class, IDocumentViewModel
        where TTool : IToolViewModel
    {
        ICollection<TTool> Tools { get; }

        ICollection<TDoc> Projects { get; }

        TDoc? CurrentProject { get; set; }
    }

    public interface IMultiProjectApp : IApp
    {
        event EventHandler CurrentProjectChanged;

        event EventHandler<NotifyCollectionChangedEventArgs> ProjectsChanged;

        DataTemplate? DocumentHeaderTemplate { get; }
    }
}