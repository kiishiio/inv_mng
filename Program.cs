using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using inv_mng;

class Program
{
    static Config config;
    static ManageItems manageItems = new ManageItems();
    static ManageConfig manageConfig = new ManageConfig();
    static Inventory manageInventory = new Inventory();
    
    public static void Init()
    {
        Console.Clear();
        config = ManageConfig.LoadConfig();
        var db = GetFolderPath($"{config.database}.db");
        InitDatabase(db);
        Menu(db);
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

    public static void Menu(string db)
    {
        Console.Clear();
        Console.WriteLine("welcome to the inventory manager!\n");
        Console.WriteLine("1. manage inventory");
        Console.WriteLine("2. display inventory");
        Console.WriteLine("3. config");
        Console.WriteLine("0. exit\n");
            
        string input = Console.ReadLine();
        switch (input)
        {
            case "0":
                Console.Clear();
                Environment.Exit(0);
                break;
            case "1":
                manageItems.Menu(db);
                break;
            case "2":
                manageInventory.ViewInventory(db);
                break;
            case "3":
                manageConfig.EditConfig(db);
                break;
            default:
                Menu(db);
                break;
        }
    }

    static void Main(string[] args)
    {
        Init();
    }
}
//TODO: make code cleaner, advanced removable items, changeable db, more modular
// there are get set thingies: private int integ = 0; public int Integ{get;set;}