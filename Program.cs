using System.Data.SQLite;
using inv_mng;

class Program
{
    private const string dbfile = "inventory_manager.db";
    private static ManageItems manageItems = new ManageItems(); //TODO: allow changes to directory

    public static string GetDatabasePath()
    {
        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "_inventory_manager_data");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, dbfile);
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
        Thread.Sleep(500);
    }

    public static void Menu(string db)
    {
        Console.Clear();
        Console.WriteLine("welcome to the inventory manager!\n");
        Console.WriteLine("1. manage inventory");
        Console.WriteLine("2. display inventory");
        Console.WriteLine("3. change database");
        Console.WriteLine("0. exit\n");
            
        string input = Console.ReadLine();
        switch (input)
        {
            case "0":
                Environment.Exit(0);
                break;
            case "1":
                manageItems.Menu(db);
                break;
            case "2":
                ViewInventory(db);
                break;
            case "3":
                break;
            default:
                Menu(db);
                break;
        }
    }

    static void ViewInventory(string db)
    {
        Console.Clear();
        using (var connection = new SQLiteConnection($"Data Source={db};Version=3"))
        {
            connection.Open();
            bool empty = Convert.ToInt32(new SQLiteCommand($"SELECT EXISTS(SELECT 1 FROM INVENTORY)", connection).ExecuteScalar()) == 0;
            if (empty)
                Console.WriteLine("inventory empty");
            else
            {
                string selectItems = "SELECT itemId, name, description, amount, category, creationDate FROM INVENTORY;";
                var command = new SQLiteCommand(selectItems, connection);
                using (var reader = command.ExecuteReader())
                {
                
                    Console.WriteLine("inventory:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["itemId"]} | name: {reader["name"]} (x{reader["amount"]}), category: {reader["category"]} - {reader["creationDate"]}");
                    }
                }
            }
            //TODO: ask for item id for closer inspection, type 0 or nothing to return
            Console.WriteLine("\npress any key to return");
            Console.ReadKey(true);
            Menu(db);
        }
    }

    static void Main(string[] args)
    {
        var db = GetDatabasePath();
        InitDatabase(db);
        Menu(db);
    }
}
//TODO: make code cleaner, removable items, saving db, changeable db, more modular, automatically remove "0 items"
// there are get set thingies: private int integ = 0; public int Integ{get;set;}