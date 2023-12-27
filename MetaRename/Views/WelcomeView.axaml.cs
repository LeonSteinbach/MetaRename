using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MetaRename.ViewModels;

namespace MetaRename.Views;

public partial class WelcomeView : UserControl
{
    public WelcomeView() {
        InitializeComponent();

        DataContext = new WelcomeViewModel();
    }
}