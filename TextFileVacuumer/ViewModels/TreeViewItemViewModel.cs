using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Reactive.Bindings.Extensions;
using TextFileVacuumer.Messaging;
using Thinva.Technology.Toolbox.Files;

namespace TextFileVacuumer.ViewModels;

[DebuggerDisplay("{ToString(),nq}")]
public sealed partial class TreeViewDirectoryViewModel : ObservableObject, IDisposable
{
    private readonly CompositeDisposable _disposables;

    public static TreeViewDirectoryViewModel Empty { get; } = new(null);

    public string Name { get; }
    public IStorageDirectory? Direcotry { get; }

    [ObservableProperty]
    bool _isSelected;

    public ObservableCollection<TreeViewDirectoryViewModel> Children { get; } = new();

    public TreeViewDirectoryViewModel(IStorageDirectory? directory)
    {
        CompositeDisposable disposables = new();
        Direcotry = directory;
        Name = directory?.FullPath ?? "";

        // チェックONでファイルのリストを追加します
        this.ObserveProperty(static x => x.IsSelected)
            .Where(b => b)
            .Subscribe(_ => PublishAddFilesRequest(Direcotry!))
            .AddTo(disposables);

        // チェックONでファイルのリストを追加します
        Children.ObserveElementProperty(static x => x.IsSelected)
            .Where(x => x.Value && x.Instance is not null)
            .Select(x => x.Instance!)
            .Subscribe(vm => PublishAddFilesRequest(vm.Direcotry!))
            .AddTo(disposables);

        // チェックONでファイルのリストを追加します
        this.ObserveProperty(static x => x.IsSelected)
            .Where(b => !b)
            .Subscribe(vm => PublishRemoveDirectoryRequest(Direcotry!))
            .AddTo(disposables);

        // チェックOFFでファイルのリストを削除します
        Children.ObserveElementProperty(static x => x.IsSelected)
            .Where(x => !x.Value && x.Instance is not null)
            .Select(x => x.Instance!)
            .Subscribe(vm => PublishRemoveDirectoryRequest(vm.Direcotry!))
            .AddTo(disposables);

        _disposables = disposables;
    }

    // ファイルリストの追加を要求します
    private static void PublishAddFilesRequest(IStorageDirectory directory)
    {
        var addFiles = directory.GetFiles().ToArray();
        WeakReferenceMessenger.Default.Send<AddTargetFilesRequest>(new(addFiles));
    }

    // ファイルリストの追加を要求します
    private static void PublishRemoveDirectoryRequest(IStorageDirectory directory)
    {
        WeakReferenceMessenger.Default.Send<RemoveTargetDirectoryRequest>(new(directory));
    }

    public void Dispose() => _disposables.Dispose();

    public override string ToString() => $"{Name}(Count={Children.Count})";
}
