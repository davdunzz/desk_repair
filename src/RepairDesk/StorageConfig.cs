using System.IO;
using System.Text.Json;

namespace RepairDesk;

public sealed class StorageOptions
{
    public string Mode { get; set; } = "PC";
    public string CustomPdfFolder { get; set; } = "";
}

public static class StorageConfig
{
    public static string ProgramFolder => AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
    private static string ConfigPath => Path.Combine(ProgramFolder, "repairdesk-storage.json");
    private static string FallbackConfigPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RepairDesk", "repairdesk-storage.json");

    public static StorageOptions Load()
    {
        foreach (var path in new[] { ConfigPath, FallbackConfigPath })
        {
            try
            {
                if (File.Exists(path)) return JsonSerializer.Deserialize<StorageOptions>(File.ReadAllText(path)) ?? new StorageOptions();
            }
            catch { }
        }
        return new StorageOptions();
    }

    public static void Save(StorageOptions options)
    {
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
        try { File.WriteAllText(ConfigPath, json); }
        catch
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FallbackConfigPath)!);
            File.WriteAllText(FallbackConfigPath, json);
        }
    }

    public static string GetDataFolder(StorageOptions? options = null)
    {
        options ??= Load();
        return options.Mode.Equals("Portable", StringComparison.OrdinalIgnoreCase)
            ? Path.Combine(ProgramFolder, "Dati")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RepairDesk");
    }

    public static string GetPdfFolder(StorageOptions? options = null)
    {
        options ??= Load();
        if (!string.IsNullOrWhiteSpace(options.CustomPdfFolder)) return options.CustomPdfFolder;
        return options.Mode.Equals("Portable", StringComparison.OrdinalIgnoreCase)
            ? Path.Combine(ProgramFolder, "PDF", "Schede PDF")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RepairDesk", "Schede PDF");
    }
}
