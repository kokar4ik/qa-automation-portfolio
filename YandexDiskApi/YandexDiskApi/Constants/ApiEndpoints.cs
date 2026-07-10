namespace YandexDiskApi.Constants;

public static class ApiEndpoints
{
    public const string Disk = "disk";
    public const string Resources = "resources";
    public const string Upload = "upload";
    public const string Move = "move";
    public const string Trash = "trash";
    public const string Restore = "restore";

    public static string DiskResources => Combine(Disk, Resources);

    public static string DiskResourcesUpload => Combine(Disk, Resources, Upload);

    public static string DiskResourcesMove => Combine(Disk, Resources, Move);

    public static string DiskTrashResources => Combine(Disk, Trash, Resources);

    public static string DiskTrashResourcesRestore => Combine(Disk, Trash, Resources, Restore);

    private static string Combine(params string[] segments) =>
        string.Join('/', segments);
}
