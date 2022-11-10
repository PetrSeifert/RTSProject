public interface IStoring
{
    int StorageSpace { get; set; } //When built adds storage space to faction

    void AddStorageSpaceToFaction();

    void RemoveStorageSpaceFromFaction();
}
