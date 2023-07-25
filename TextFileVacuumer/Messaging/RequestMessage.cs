using Thinva.Technology.Toolbox.Files;

namespace TextFileVacuumer.Messaging;

internal sealed record AddTargetFilesRequest(IReadOnlyList<IStorageFile> AddFiles);

internal sealed record RemoveTargetDirectoryRequest(IStorageDirectory RemoveDirectory);
