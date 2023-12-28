using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Platform.Storage;
using MetaRename.Views;
using ReactiveUI;

namespace MetaRename.ViewModels;

public class WelcomeViewModel : ViewModelBase
{
    public ICommand OpenFolderCommand { get; }

    private bool includeSubfolders;

    public bool IncludeSubfolders {
        get => includeSubfolders;
        set {
            includeSubfolders = value;
            this.RaiseAndSetIfChanged(ref includeSubfolders, value);
        }
    }

    public bool AreFoldersSelected => SelectedFolders.Count > 0;

    private ObservableCollection<Uri> selectedFolders = [];
    
    public ObservableCollection<Uri> SelectedFolders {
        get => selectedFolders;
        set {
            selectedFolders = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(AreFoldersSelected));
        }
    }

    public WelcomeViewModel() {
        SelectedFolders = [];
        
        OpenFolderCommand = ReactiveCommand.Create(async () => {
            var options = new FolderPickerOpenOptions {
                AllowMultiple = true
            };
            var folders = await MainWindow.Instance.StorageProvider.OpenFolderPickerAsync(options);
            SelectedFolders = new ObservableCollection<Uri>(folders.Select(folder => folder.Path));
        });
    }
}