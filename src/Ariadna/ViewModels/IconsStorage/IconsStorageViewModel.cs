using Ariadna.Core;
using Microsoft.Win32;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Ariadna
{
    internal class IconsStorageViewModel : BaseViewModel
    {
        #region Private Methods

        private readonly IconsStorageDialog _iconsStorageDialog;
        private bool _isSelect;

        private DirectoryInfo _iconDirectory;
        private DirectoryInfo _currentDirectory;
        private readonly AriadnaApp _akimApp;

        #endregion

        #region Public Properties

        public ObservableCollection<IconViewModel> Items { get; set; } =
            new ObservableCollection<IconViewModel>();

        public IconViewModel SelectedItem { get; set; }

        #endregion

        #region Commands

        public DelegateCommand CancelCommand { get; }
        public DelegateCommand SelectCommand { get; }
        public DelegateCommand AddIconCommand { get; }

        public DelegateCommand OpenFolderOrCheckIconCommand { get; }
        public DelegateCommand AddFolderCommand { get; }

        #endregion

        #region Constructor

        public IconsStorageViewModel(IconsStorageDialog iconsStorageDialog, AriadnaApp akimApp)
        {
            _iconsStorageDialog = iconsStorageDialog;
            _akimApp = akimApp;

            CancelCommand = new DelegateCommand(Cancel);
            SelectCommand = new DelegateCommand(Select, CanSelect);
            AddIconCommand = new DelegateCommand(AddIcon);

            OpenFolderOrCheckIconCommand = new DelegateCommand(OpenFolderOrCheckIcon);
            AddFolderCommand = new DelegateCommand(AddFolder);


            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedItem))
                    SelectCommand.RaiseCanExecuteChanged();
            };
        }

        #endregion

        #region Public Methods

        public string Load()
        {
            var basePath = _akimApp.GetRootDirectory();

            _iconDirectory = basePath.CreateSubdirectory("Icons");
            _currentDirectory = _iconDirectory;


            foreach (var directoryInfo in _iconDirectory.GetDirectories())
            {
                Items.Add(new IconViewModel(directoryInfo));
            }

            foreach (var fileInfo in _iconDirectory.GetFiles())
            {
                if (IsIcon(fileInfo.FullName))
                    Items.Add(new IconViewModel(fileInfo, _iconDirectory, _iconDirectory, _akimApp));
            }

            _iconsStorageDialog.ShowDialog();

            return _isSelect ? SelectedItem.RelativePath : null;
        }

        #endregion

        #region Private Methods

        private bool CanSelect() => SelectedItem != null && SelectedItem.Type == ItemType.Icon;

        private void Cancel()
        {
            _iconsStorageDialog.Close();
        }

        private void Select()
        {
            _isSelect = true;

            _iconsStorageDialog.Close();
        }

        private void AddIcon()
        {
            var filter = ImageHelpers.GenerateImageFilter();

            var openFileDialog = new OpenFileDialog
            {
                Title = "Выбрать файл изображения",
                Filter = filter,
                DefaultExt = ".png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var path = new FileInfo(openFileDialog.FileName);

                var newPath = Path.Combine(_currentDirectory.FullName, path.Name);

                path.CopyTo(newPath);

                Items.Add(new IconViewModel(new FileInfo(newPath), _currentDirectory, _iconDirectory, _akimApp));
            }
        }
        
        private void AddFolder()
        {
            var filters = new List<string>();

            foreach (var directoryInfo in _currentDirectory.GetDirectories())
                filters.Add(directoryInfo.Name);

            var tbDialog = new TextBoxDialog
            {
                Title = "Добавить каталог",
                Text = "Новый каталог",
                ValidationFilters = filters
            };

            tbDialog.SelectAll();

            if (tbDialog.ShowDialog() == true)
            {
                var folderName = tbDialog.Text;

                var newDirectoryInfo = _currentDirectory.CreateSubdirectory(folderName);

                Items.Add(new IconViewModel(newDirectoryInfo));
            }
        }

        private void OpenFolderOrCheckIcon()
        {
            if (SelectedItem.Type == ItemType.Icon)
                Select();
            else if (SelectedItem.Type == ItemType.Directory)
                OpenFolder();
            else
                Back();
        }

        private void Back()
        {
            OpenFolder();
        }

        private void OpenFolder()
        {
            var directoryInfo = SelectedItem.Directory;

            _currentDirectory = directoryInfo;

            Items.Clear();

            if (_currentDirectory.FullName != _iconDirectory.FullName)
                Items.Add(new IconViewModel("..", _currentDirectory.Parent));

            foreach (var directory in _currentDirectory.GetDirectories())
            {
                Items.Add(new IconViewModel(directory));
            }

            foreach (var fileInfo in _currentDirectory.GetFiles())
            {
                if (IsIcon(fileInfo.FullName))
                    Items.Add(new IconViewModel(fileInfo, _currentDirectory, _iconDirectory, _akimApp));
            }
        }

        private static bool IsIcon(string path)
        {
            var validFormats = new[]
            {
                "BMP", "DIB", "RLE",
                "JPG", "JPEG", "JPE", "JFIF",
                "GIF",
                "TIF", "TIFF",
                "PNG",
                "SVG",
                "ICO"
            };

            foreach (var validFormat in validFormats)
            {
                if (path.ToUpper().EndsWith(validFormat))
                    return true;
            }

            return false;
        }

        #endregion
    }
}