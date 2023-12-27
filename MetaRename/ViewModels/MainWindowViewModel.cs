using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MetaRename.Views;
using ReactiveUI;

namespace MetaRename.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ICommand OpenFolderCommand { get; }

    public MainWindowViewModel() {
        OpenFolderCommand = ReactiveCommand.Create(async () => {
            var options = new FolderPickerOpenOptions { AllowMultiple = true };
            var folders = await MainWindow.Instance.StorageProvider.OpenFolderPickerAsync(options);
            foreach (var folder in folders) {
                Console.WriteLine(folder.Path);
            }
        });
    }
}