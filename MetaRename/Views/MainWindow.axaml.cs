using Avalonia.Controls;

namespace MetaRename.Views;

public partial class MainWindow : Window
{
    public static Window Instance { get; private set; }
    
    public MainWindow() {
        InitializeComponent();

        Instance = this;
    }
}