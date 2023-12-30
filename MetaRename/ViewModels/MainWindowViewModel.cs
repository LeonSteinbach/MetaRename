using System;
using System.Windows.Input;
using Avalonia.Controls;
using MetaRename.Views;
using ReactiveUI;

namespace MetaRename.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private UserControl? content;

    public UserControl? Content {
        get => content;
        set {
            content = value;
            this.RaisePropertyChanged();
        }
    }

    public ICommand BackViewCommand { get; }
    
    public ICommand ContinueViewCommand { get; }

    public MainWindowViewModel() {
        content = new SelectFoldersView();
        
        BackViewCommand = ReactiveCommand.Create(() => {
            Type? contentType = Content?.GetType();
            
            switch (contentType) {
                case not null when contentType == typeof(SelectFoldersView):
                    break;
                case not null when contentType == typeof(FilterFilesView):
                    Content = new SelectFoldersView();
                    break;
                case not null when contentType == typeof(RenameFilesView):
                    Content = new FilterFilesView();
                    break;
            }
        });
        
        ContinueViewCommand = ReactiveCommand.Create(() => {
            Type? contentType = Content?.GetType();
            
            switch (Content) {
                case not null when contentType == typeof(SelectFoldersView):
                    Content = new FilterFilesView();
                    break;
                case not null when contentType == typeof(FilterFilesView):
                    Content = new RenameFilesView();
                    break;
                case not null when contentType == typeof(RenameFilesView):
                    break;
            }
        });
    }
}