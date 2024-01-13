using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DynamicData;
using MetaRename.Views;
using ReactiveUI;

namespace MetaRename.ViewModels;

public class MainViewModel : ViewModelBase
{
    public bool ShowBackViewButton => Content?.GetType() != typeof(SelectFoldersView);

    private UserControl? selectFoldersView;
    private UserControl? filterFilesView;
    private UserControl? renameFilesView;
    
    private UserControl? content;

    public UserControl? Content {
        get => content;
        set {
            content = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(ShowBackViewButton));
        }
    }

    public ICommand BackViewCommand { get; }
    
    public ICommand ContinueViewCommand { get; }
    
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
            FilterFiles();
        }
    }

    private string filterFilesText = string.Empty;

    public string FilterFilesText {
        get => filterFilesText;
        set {
            filterFilesText = value;
            this.RaisePropertyChanged();
            FilterFiles();
        }
    }

    private ObservableCollection<Uri> filteredFiles;

    public ObservableCollection<Uri> FilteredFiles {
        get => filteredFiles;
        set {
            filteredFiles = value;
            this.RaisePropertyChanged();
        }
    }

    private void FilterFiles() {
        FilteredFiles = [];
        SearchOption searchOption = IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        foreach (Uri folder in SelectedFolders) {
            foreach (string file in Directory.GetFiles(folder.LocalPath, FilterFilesText, searchOption)) {
                FilteredFiles.Add(new Uri(file));
            }
        }
    }

    public MainViewModel() {
        selectFoldersView = new SelectFoldersView();
        filterFilesView = new FilterFilesView();
        renameFilesView = new RenameFilesView();
        
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