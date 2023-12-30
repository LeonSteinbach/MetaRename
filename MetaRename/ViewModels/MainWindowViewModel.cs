using System;
using System.Windows.Input;
using Avalonia.Controls;
using MetaRename.Views;
using ReactiveUI;

namespace MetaRename.ViewModels;

public class MainWindowViewModel : ViewModelBase
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

    public MainWindowViewModel() {
        selectFoldersView = new SelectFoldersView();
        filterFilesView = new FilterFilesView();
        renameFilesView = new RenameFilesView();
        
        content = selectFoldersView;
        
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
    }
}