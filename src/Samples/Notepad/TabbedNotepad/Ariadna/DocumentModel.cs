using System.IO;
using System.Windows;
using System.Windows.Media;
using Ariadna;
using TabbedNotepad.Controls;

namespace TabbedNotepad;

internal class DocumentModel : IDocumentViewModel
{
    public string Content { get; set; }

    public bool HaveChanges { get; set; }

    public DocumentModel(string docPath)
    {
        var fileInfo = new FileInfo(docPath);

        Title = fileInfo.Name;
        Content = File.ReadAllText(docPath);
    }

    public string Title { get; set; }
    public ImageSource IconSource { get; set; }
    public string ContentId { get; set; }
    public bool IsSelected { get; set; }
    public bool IsActive { get; set; }

    public DataTemplate GetTemplate() => new(typeof(DocumentModel))
    {
        VisualTree = new FrameworkElementFactory(typeof(NotepadControl))
    };

    public void Dispose()
    {
    }
}