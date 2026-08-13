using Microsoft.Data.Sqlite;
using System.IO;
using System.Text.Json;

namespace RepairDesk;

public static class Database
{
    private static string DataFolder => StorageConfig.GetDataFolder();
    public static string DbPath => Path.Combine(DataFolder, "repairdesk.db");
    private static string ConnectionString => $"Data Source={DbPath}";

    public static void Initialize()
    {
        Directory.CreateDirectory(DataFolder);
        using var connection = Open();
        var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Brands (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL UNIQUE COLLATE NOCASE);
            CREATE TABLE IF NOT EXISTS PhoneModels (Id INTEGER PRIMARY KEY AUTOINCREMENT, BrandId INTEGER NOT NULL, Name TEXT NOT NULL COLLATE NOCASE, UNIQUE(BrandId, Name), FOREIGN KEY(BrandId) REFERENCES Brands(Id));
            CREATE TABLE IF NOT EXISTS Repairs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, PracticeNumber TEXT NOT NULL UNIQUE, CreatedAt TEXT NOT NULL,
                FirstName TEXT NOT NULL, LastName TEXT NOT NULL, Phone TEXT NOT NULL, Email TEXT,
                Brand TEXT NOT NULL, Model TEXT NOT NULL, Color TEXT, Imei TEXT, RepairDescription TEXT NOT NULL,
                RepairTypes TEXT NOT NULL, Accessories TEXT NOT NULL, DeviceConditions TEXT NOT NULL, ConditionNotes TEXT
            );
            CREATE TABLE IF NOT EXISTS Settings (Key TEXT PRIMARY KEY, Value TEXT NOT NULL);
            """;
        command.ExecuteNonQuery();
        EnsureColumn(connection, "Repairs", "AppointmentAt", "TEXT NULL");
        SeedCatalog(connection);
    }

    public static void SwitchStorage(StorageOptions newOptions)
    {
        var currentPath = DbPath;
        var targetFolder = StorageConfig.GetDataFolder(newOptions);
        var targetPath = Path.Combine(targetFolder, "repairdesk.db");
        Directory.CreateDirectory(targetFolder);
        if (!Path.GetFullPath(currentPath).Equals(Path.GetFullPath(targetPath), StringComparison.OrdinalIgnoreCase) && File.Exists(currentPath))
        {
            if (File.Exists(targetPath)) File.Copy(targetPath, targetPath + $".backup-{DateTime.Now:yyyyMMdd-HHmmss}", true);
            File.Copy(currentPath, targetPath, true);
        }
        StorageConfig.Save(newOptions);
        Initialize();
    }

    private static void EnsureColumn(SqliteConnection connection, string table, string column, string definition)
    {
        using var info = connection.CreateCommand();
        info.CommandText = $"PRAGMA table_info({table})";
        using var reader = info.ExecuteReader();
        while (reader.Read()) if (reader.GetString(1).Equals(column, StringComparison.OrdinalIgnoreCase)) return;
        reader.Close();
        using var alter = connection.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition}";
        alter.ExecuteNonQuery();
    }

    private static SqliteConnection Open()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    private static void SeedCatalog(SqliteConnection connection)
    {
        using var count = connection.CreateCommand();
        count.CommandText = "SELECT COUNT(*) FROM Brands";
        if (Convert.ToInt32(count.ExecuteScalar()) > 0) return;

        using var transaction = connection.BeginTransaction();
        foreach (var item in CatalogSeed.Data)
        {
            using var brand = connection.CreateCommand();
            brand.Transaction = transaction;
            brand.CommandText = "INSERT INTO Brands(Name) VALUES($name); SELECT last_insert_rowid();";
            brand.Parameters.AddWithValue("$name", item.Key);
            var brandId = Convert.ToInt64(brand.ExecuteScalar());
            foreach (var modelName in item.Value)
            {
                using var model = connection.CreateCommand();
                model.Transaction = transaction;
                model.CommandText = "INSERT INTO PhoneModels(BrandId, Name) VALUES($brandId, $name)";
                model.Parameters.AddWithValue("$brandId", brandId);
                model.Parameters.AddWithValue("$name", modelName);
                model.ExecuteNonQuery();
            }
        }
        transaction.Commit();
    }

    public static List<string> GetBrands()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Name FROM Brands ORDER BY CASE WHEN Name='Altro' THEN 1 ELSE 0 END, Name";
        using var reader = command.ExecuteReader();
        var result = new List<string>();
        while (reader.Read()) result.Add(reader.GetString(0));
        return result;
    }

    public static List<string> GetModels(string brand)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT m.Name FROM PhoneModels m JOIN Brands b ON b.Id=m.BrandId WHERE b.Name=$brand ORDER BY m.Name";
        command.Parameters.AddWithValue("$brand", brand);
        using var reader = command.ExecuteReader();
        var result = new List<string>();
        while (reader.Read()) result.Add(reader.GetString(0));
        return result;
    }

    public static void AddBrand(string name)
    {
        name = name.Trim();
        if (name.Length == 0) return;
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT OR IGNORE INTO Brands(Name) VALUES($name)";
        command.Parameters.AddWithValue("$name", name);
        command.ExecuteNonQuery();
    }

    public static void AddModel(string brand, string model)
    {
        AddBrand(brand);
        model = model.Trim();
        if (model.Length == 0) return;
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT OR IGNORE INTO PhoneModels(BrandId,Name) SELECT Id,$model FROM Brands WHERE Name=$brand COLLATE NOCASE";
        command.Parameters.AddWithValue("$brand", brand.Trim());
        command.Parameters.AddWithValue("$model", model);
        command.ExecuteNonQuery();
    }

    public static int SaveRepair(RepairRecord item)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Repairs(PracticeNumber,CreatedAt,AppointmentAt,FirstName,LastName,Phone,Email,Brand,Model,Color,Imei,RepairDescription,RepairTypes,Accessories,DeviceConditions,ConditionNotes)
            VALUES($practice,$created,$appointment,$first,$last,$phone,$email,$brand,$model,$color,$imei,$description,$types,$accessories,$conditions,$notes);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$practice", item.PracticeNumber);
        command.Parameters.AddWithValue("$created", item.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("$appointment", item.AppointmentAt?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$first", item.FirstName);
        command.Parameters.AddWithValue("$last", item.LastName);
        command.Parameters.AddWithValue("$phone", item.Phone);
        command.Parameters.AddWithValue("$email", item.Email);
        command.Parameters.AddWithValue("$brand", item.Brand);
        command.Parameters.AddWithValue("$model", item.Model);
        command.Parameters.AddWithValue("$color", item.Color);
        command.Parameters.AddWithValue("$imei", item.Imei);
        command.Parameters.AddWithValue("$description", item.RepairDescription);
        command.Parameters.AddWithValue("$types", JsonSerializer.Serialize(item.RepairTypes));
        command.Parameters.AddWithValue("$accessories", JsonSerializer.Serialize(item.Accessories));
        command.Parameters.AddWithValue("$conditions", JsonSerializer.Serialize(item.DeviceConditions));
        command.Parameters.AddWithValue("$notes", item.ConditionNotes);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public static void UpdateRepair(RepairRecord item)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Repairs SET AppointmentAt=$appointment,FirstName=$first,LastName=$last,Phone=$phone,Email=$email,Brand=$brand,Model=$model,
            Color=$color,Imei=$imei,RepairDescription=$description,RepairTypes=$types,Accessories=$accessories,DeviceConditions=$conditions,ConditionNotes=$notes
            WHERE Id=$id
            """;
        AddRepairParameters(command, item);
        command.Parameters.AddWithValue("$id", item.Id);
        command.ExecuteNonQuery();
    }

    private static void AddRepairParameters(SqliteCommand command, RepairRecord item)
    {
        command.Parameters.AddWithValue("$appointment", item.AppointmentAt?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$first", item.FirstName); command.Parameters.AddWithValue("$last", item.LastName);
        command.Parameters.AddWithValue("$phone", item.Phone); command.Parameters.AddWithValue("$email", item.Email);
        command.Parameters.AddWithValue("$brand", item.Brand); command.Parameters.AddWithValue("$model", item.Model);
        command.Parameters.AddWithValue("$color", item.Color); command.Parameters.AddWithValue("$imei", item.Imei);
        command.Parameters.AddWithValue("$description", item.RepairDescription);
        command.Parameters.AddWithValue("$types", JsonSerializer.Serialize(item.RepairTypes));
        command.Parameters.AddWithValue("$accessories", JsonSerializer.Serialize(item.Accessories));
        command.Parameters.AddWithValue("$conditions", JsonSerializer.Serialize(item.DeviceConditions));
        command.Parameters.AddWithValue("$notes", item.ConditionNotes);
    }

    public static void UpdateAppointment(int id, DateTime? appointment)
    {
        using var connection = Open(); using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Repairs SET AppointmentAt=$appointment WHERE Id=$id";
        command.Parameters.AddWithValue("$appointment", appointment?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$id", id); command.ExecuteNonQuery();
    }

    public static void DeleteRepair(int id)
    {
        using var connection = Open(); using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Repairs WHERE Id=$id"; command.Parameters.AddWithValue("$id", id); command.ExecuteNonQuery();
    }

    public static List<RepairRecord> SearchRepairs(string search = "")
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id,PracticeNumber,CreatedAt,FirstName,LastName,Phone,Email,Brand,Model,Color,Imei,RepairDescription,RepairTypes,Accessories,DeviceConditions,ConditionNotes,AppointmentAt
            FROM Repairs WHERE $q='' OR PracticeNumber LIKE $like OR FirstName LIKE $like OR LastName LIKE $like OR Phone LIKE $like OR Email LIKE $like OR Imei LIKE $like
            ORDER BY Id DESC LIMIT 500
            """;
        command.Parameters.AddWithValue("$q", search.Trim());
        command.Parameters.AddWithValue("$like", $"%{search.Trim()}%");
        using var reader = command.ExecuteReader();
        var result = new List<RepairRecord>();
        while (reader.Read()) result.Add(ReadRepair(reader));
        return result;
    }

    private static RepairRecord ReadRepair(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(0), PracticeNumber = r.GetString(1), CreatedAt = DateTime.Parse(r.GetString(2)),
        FirstName = r.GetString(3), LastName = r.GetString(4), Phone = r.GetString(5), Email = r.GetString(6),
        Brand = r.GetString(7), Model = r.GetString(8), Color = r.GetString(9), Imei = r.GetString(10), RepairDescription = r.GetString(11),
        RepairTypes = JsonSerializer.Deserialize<List<string>>(r.GetString(12)) ?? [], Accessories = JsonSerializer.Deserialize<List<string>>(r.GetString(13)) ?? [],
        DeviceConditions = JsonSerializer.Deserialize<List<string>>(r.GetString(14)) ?? [], ConditionNotes = r.GetString(15),
        AppointmentAt = r.IsDBNull(16) ? null : DateTime.Parse(r.GetString(16))
    };

    public static List<RepairRecord> GetAppointments(DateTime day) => SearchRepairs()
        .Where(x => x.AppointmentAt?.Date == day.Date).OrderBy(x => x.AppointmentAt).ToList();

    public static string NextPracticeNumber() => $"R-{DateTime.Now:yyyyMMdd-HHmmssfff}";

    public static ShopSettings LoadSettings()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Value FROM Settings WHERE Key='shop'";
        var json = command.ExecuteScalar()?.ToString();
        return string.IsNullOrWhiteSpace(json) ? new ShopSettings() : JsonSerializer.Deserialize<ShopSettings>(json) ?? new ShopSettings();
    }

    public static void SaveSettings(ShopSettings settings)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Settings(Key,Value) VALUES('shop',$value) ON CONFLICT(Key) DO UPDATE SET Value=$value";
        command.Parameters.AddWithValue("$value", JsonSerializer.Serialize(settings));
        command.ExecuteNonQuery();
    }
}
