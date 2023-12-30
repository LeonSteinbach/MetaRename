using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetaRename.ViewModels;

namespace MetaRename.Views;

public partial class SelectFoldersView : UserControl
{
    public SelectFoldersView() {
        InitializeComponent();

        DataContext = new SelectFoldersViewModel();
    }
}