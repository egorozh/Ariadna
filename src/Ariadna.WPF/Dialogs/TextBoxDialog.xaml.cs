using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ariadna;

public partial class TextBoxDialog
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(TextBoxDialog),
        new PropertyMetadata(default(string)));


    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty ValidationFiltersProperty = DependencyProperty.Register(
        "ValidationFilters", typeof(ICollection<string>), typeof(TextBoxDialog),
        new PropertyMetadata(default(ICollection<string>)));

    public ICollection<string> ValidationFilters
    {
        get => (ICollection<string>) GetValue(ValidationFiltersProperty);
        set => SetValue(ValidationFiltersProperty, value);
    }

    public TextBoxDialog()
    {
        InitializeComponent();

        Owner = Application.Current.MainWindow;
        ShowInTaskbar = false;
    }

    public void SelectAll()
    {
        TextBox.Focus();
        TextBox.SelectAll();
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }

    private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        this.Close();
    }

    private void TextBoxDialog_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && CanOk())
        {
            this.DialogResult = true;
            this.Close();
        }
        else if (e.Key == Key.Escape)
        {
            this.DialogResult = false;
            this.Close();
        }
    }

    private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        OkButton.IsEnabled = CanOk();
    }

    private bool CanOk()
    {
        var text = TextBox.Text;

        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        if (ValidationFilters != null)
        {
            foreach (var validationFilter in ValidationFilters)
            {
                if (text == validationFilter)
                    return false;
            }
        }

        return true;
    }
}