using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using MetaRename.Views;
using ReactiveUI;

namespace MetaRename.ViewModels;

public class MainViewModel : ViewModelBase
{
    public bool ShowBackViewButton => Content?.GetType() != typeof(SelectFoldersView);
    public bool ShowContinueViewButton => Content?.GetType() != typeof(RenameFilesView);
    public bool ShowRenameButtons => Content?.GetType() == typeof(RenameFilesView);

    private UserControl? content;

    public UserControl? Content {
        get => content;
        set {
            this.RaiseAndSetIfChanged(ref content, value);
            this.RaisePropertyChanged(nameof(ShowBackViewButton));
            this.RaisePropertyChanged(nameof(ShowContinueViewButton));
            this.RaisePropertyChanged(nameof(ShowRenameButtons));
        }
    }

    public ICommand BackViewCommand { get; }

    public ICommand ContinueViewCommand { get; }

    public ICommand OpenFolderCommand { get; }

    private bool includeSubfolders;

    public bool IncludeSubfolders {
        get => includeSubfolders;
        set {
            this.RaiseAndSetIfChanged(ref includeSubfolders, value);
            FilterFiles();
        }
    }

    public bool AreFoldersSelected => SelectedFolders.Count > 0;

    private ObservableCollection<Uri> selectedFolders = [];

    public ObservableCollection<Uri> SelectedFolders {
        get => selectedFolders;
        set {
            this.RaiseAndSetIfChanged(ref selectedFolders, value);
            this.RaisePropertyChanged(nameof(AreFoldersSelected));
            FilterFiles();
        }
    }

    private string filterFilesText = string.Empty;

    public string FilterFilesText {
        get => filterFilesText;
        set {
            this.RaiseAndSetIfChanged(ref filterFilesText, value);
            FilterFiles();
        }
    }

    private ObservableCollection<Uri> filteredFiles = [];

    public ObservableCollection<Uri> FilteredFiles {
        get => filteredFiles;
        set => this.RaiseAndSetIfChanged(ref filteredFiles, value);
    }

    private IBrush filterFilesTextColor = Brushes.White;

    public IBrush FilterFilesTextColor {
        get => filterFilesTextColor;
        set => this.RaiseAndSetIfChanged(ref filterFilesTextColor, value);
    }

    public static ObservableCollection<string> AvailablePatterns => [
        "yyyyMMdd", "HHmmss", "Document Name", "document name", "DOCUMENT NAME", "Camera manufacturer", "Camera model",
        "1 digit number", "2 digit number", "3 digit number", "4 digit number", "5 digit number", "6 digit number"
    ];

    private void FilterFiles() {
        FilteredFiles = [];
        SearchOption searchOption = IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        Regex? regex = null;
        try {
            regex = new Regex(FilterFilesText);
            FilterFilesTextColor = Brushes.White;
        }
        catch (RegexParseException) {
            FilterFilesTextColor = Brushes.Red;
        }
        catch (Exception) {
            // ignored
        }

        foreach (Uri folder in SelectedFolders) {
            foreach (string file in Directory.GetFiles(folder.LocalPath, "*", searchOption)) {
                if (regex != null && regex.IsMatch(file)) {
                    FilteredFiles.Add(new Uri(file));
                }
            }
        }
    }

    public MainViewModel() {
        UserControl selectFoldersView = new SelectFoldersView();
        UserControl filterFilesView = new FilterFilesView();
        UserControl renameFilesView = new RenameFilesView();

        content = selectFoldersView;

        SelectedFolders = [];
        FilteredFiles = [];

        BackViewCommand = ReactiveCommand.Create(() => {
            Type? contentType = Content?.GetType();

            Content = contentType switch {
                _ when contentType == typeof(SelectFoldersView) => Content,
                _ when contentType == typeof(FilterFilesView) => selectFoldersView,
                _ when contentType == typeof(RenameFilesView) => filterFilesView,
                _ => Content
            };
        });

        ContinueViewCommand = ReactiveCommand.Create(() => {
            Type? contentType = Content?.GetType();

            Content = contentType switch {
                _ when contentType == typeof(SelectFoldersView) => filterFilesView,
                _ when contentType == typeof(FilterFilesView) => renameFilesView,
                _ when contentType == typeof(RenameFilesView) => Content,
                _ => Content
            };
        });

        OpenFolderCommand = ReactiveCommand.Create(async () => {
            var options = new FolderPickerOpenOptions {
                AllowMultiple = true
            };
            var folders = await MainWindow.Instance.StorageProvider.OpenFolderPickerAsync(options);
            SelectedFolders = new ObservableCollection<Uri>(folders.Select(folder => folder.Path));
        });
    }
}