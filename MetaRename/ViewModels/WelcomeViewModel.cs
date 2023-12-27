using System;
using System.Windows.Input;
using Avalonia.Platform.Storage;
using MetaRename.Views;
using ReactiveUI;

namespace MetaRename.ViewModels;

public class WelcomeViewModel : ViewModelBase
{
    public ICommand OpenFolderCommand { get; }

    public WelcomeViewModel() {
        Console.WriteLine("1");
        OpenFolderCommand = ReactiveCommand.Create(async () => {
            Console.WriteLine("test");
            var options = new FolderPickerOpenOptions {
                AllowMultiple = true
            };
            var folders = await MainWindow.Instance.StorageProvider.OpenFolderPickerAsync(options);
            foreach (var folder in folders) {
                Console.WriteLine(folder.Path);
            }
        });
    }
}