using System.IO;
using System.Windows;
using System.Windows.Markup;
using Ariadna.Core;

namespace Ariadna
{
    internal class IconViewModel : BaseViewModel
    {
        public FileInfo IconFilePath { get; }
        public DirectoryInfo Directory { get; }


        public FrameworkElement Icon { get; set; }

        public string Header { get; set; }

        public string RelativePath { get; set; }

        public ItemType Type { get; }

        public IconViewModel(DirectoryInfo directory)
        {
            Directory = directory;
            Type = ItemType.Directory;
            Header = directory.Name;

            Icon = GetFolderIcon();
        }

        public IconViewModel(FileInfo iconFilePath, DirectoryInfo directory, DirectoryInfo rootDirectory,
            AriadnaApp akimApp)
        {
            IconFilePath = iconFilePath;
            Type = ItemType.Icon;

            Header = iconFilePath.Name;

            var prevRelativePath =
                Path.Combine(directory.FullName.Replace(rootDirectory.FullName, ""), iconFilePath.Name);

            if (prevRelativePath.StartsWith("\\"))
                prevRelativePath = prevRelativePath.Substring(1, prevRelativePath.Length - 1);

            RelativePath = prevRelativePath;
            
            Icon = akimApp.InterfaceHelper.GetIcon(iconFilePath.FullName);
        }

        public IconViewModel(string header, DirectoryInfo directory)
        {
            Type = ItemType.Back;
            Header = header;
            Directory = directory;

            Icon = GetFolderIcon();
        }

        private static System.Windows.Shapes.Path GetFolderIcon()
        {
            const string pathStroke = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
                                      "Fill=\"DarkOrange\" Stretch=\"Uniform\" " +
                                      "Data=\"" +
                                      "M19,20H4C2.89,20 2,19.1 2,18V6C2,4.89 2.89,4 4,4H10L12,6H19A2,2 0 0,1 21,8H21L4,8V18L6.14,10H23.21L20.93,18.5C20.7,19.37 19.92,20 19,20Z\"" +
                                      "/>";

            var path = (System.Windows.Shapes.Path) XamlReader.Parse(pathStroke);

            return path;
        }
    }

    internal enum ItemType
    {
        Directory,
        Icon,
        Back
    }
}