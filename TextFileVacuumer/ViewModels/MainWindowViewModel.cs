using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Reactive.Bindings.Extensions;
using TextFileVacuumer.Messaging;
using Thinva.Technology.Toolbox.Collections.Extensions;
using Thinva.Technology.Toolbox.Files;

namespace TextFileVacuumer.ViewModels;

public sealed partial class MainWindowViewModel : ObservableRecipient
{
    [ObservableProperty]
    IStorageDirectory? _rootDirectory;

    [ObservableProperty]
    IStorageDirectory? _saveDirectory;

    [ObservableProperty]
    string? _rootDirectoryText;
    partial void OnRootDirectoryTextChanged(string? value) => RootDirectory = StorageDirectory.TryCreate(value);

    [ObservableProperty]
    string? _saveDirectoryText;
    partial void OnSaveDirectoryTextChanged(string? value) => SaveDirectory = StorageDirectory.TryCreate(value);

    public ObservableCollection<TreeViewDirectoryViewModel> DirectoryViewModels { get; } = new();
    public ObservableCollection<IStorageFile> TargetFiles { get; } = new();

    public MainWindowViewModel()
    {
        this.ObserveProperty(static x => x.RootDirectory)
            .Subscribe(x => Debug.WriteLine(x));

        RootDirectoryText = @"D:\data";

        DirectoryViewModels.AddRange(GetDirectoryViewModels(RootDirectory));

        IsActive = true;
    }

    private static IEnumerable<TreeViewDirectoryViewModel> GetDirectoryViewModels(IStorageDirectory? rootDirectory)
    {
        if (rootDirectory is not null)
            yield return getTreeViewModel(rootDirectory);

        static TreeViewDirectoryViewModel getTreeViewModel(IStorageDirectory parentDirectory)
        {
            TreeViewDirectoryViewModel parent = new(parentDirectory);

            foreach (var childDirectory in parentDirectory.GetDirectories())
            {
                TreeViewDirectoryViewModel vm = getTreeViewModel(childDirectory);
                parent.Children.Add(vm);
            }
            return parent;
        }
    }

    protected override void OnActivated()
    {
        Messenger.Register<MainWindowViewModel, AddTargetFilesRequest>(this, static (r, m) =>
        {
            var files = new List<IStorageFile>(r.TargetFiles);

            foreach (var addFile in m.AddFiles)
            {
                if (!files.Contains(addFile))
                    files.Add(addFile);
            }
            r.TargetFiles.Clear();
            r.TargetFiles.AddRange(files.OrderBy(x => x.FullPath, StringComparer.Ordinal));
        });

        Messenger.Register<MainWindowViewModel, RemoveTargetDirectoryRequest>(this, static (r, m) =>
        {
            var removeFiles = new List<IStorageFile>();
            foreach (var removeFile in r.TargetFiles)
            {
                if (m.RemoveDirectory.Equals(removeFile.Directory))
                    removeFiles.Add(removeFile);
            }
            removeFiles.ForEach(f => r.TargetFiles.Remove(f));
        });
    }
}
