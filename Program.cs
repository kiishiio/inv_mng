using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using inv_mng;

class Program
{
    static Config config;
    static ManageConfig _config = new ManageConfig();
    static Inventory _inventory = new Inventory();
    static Items _items = new Items();
    
    public static void Init()
    {
        Console.Clear();
        config = ManageConfig.LoadConfig();
        var db = GetFolderPath($"{config.database}.db");
        InitDatabase(db);
        Console.Clear();
        
        _inventory.ViewInventory(db);
        
        Console.WriteLine("\n:manage inventory:");
        Console.WriteLine("1. add item");
        Console.WriteLine("2. delete item");
        Console.WriteLine("3. inspect item");
        Console.WriteLine("4. config");
        Console.WriteLine("0. exit");

        while (true)
        {
            string k = Console.ReadLine();
            switch (k)
            {
                case "0":
                    Console.Clear();
                    Environment.Exit(0);
                    break;
                case "1":
                    _items.AddItem(db);
                    break;
                case "2":
                    _items.RemoveItem(db);
                    break;
                case "3":
                    //wasnt able to get this done
                    break;
                case "4":
                    _config.EditConfig(db);
                    break;
                default:
                    break;
            }
        }
    }
    
    public static string GetFolderPath(string file)
    {
        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "_inventory_manager_data");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, file);
    }
    
    static void InitDatabase(string db)
    {
        if (!File.Exists(db))
        {
            SQLiteConnection.CreateFile(db);
            using (var connection = new SQLiteConnection($"Data Source={db};Version=3"))
            {
                connection.Open();

                string createInventoryTable = @"
                CREATE TABLE IF NOT EXISTS INVENTORY (
                    itemId INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    description TEXT NOT NULL,
                    amount INTEGER NOT NULL,
                    category TEXT NOT NULL,
                    creationDate TEXT NOT NULL    
                );";

                var command = new SQLiteCommand(createInventoryTable, connection);
                command.ExecuteNonQuery();
            }
            Console.WriteLine($"database created ({db})\n");
        }
        else Console.WriteLine($"database found ({db})\n");
        Thread.Sleep(800);
    }

    static void Main(string[] args)
    {
        Init();
    }
}