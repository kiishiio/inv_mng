using System.Text.Json;
using System.Text.RegularExpressions;

namespace inv_mng;

public class Config
{
    public string database { get; set; }
}

public class ManageConfig
{
    private const string config = "config.json";
    
    public static Config LoadConfig()
    {
        if (!File.Exists(Program.GetFolderPath(config)))
        {
            var defaultConfig = new Config
            {
                database = "inventory_manager"
            };
            
            SaveConfig(defaultConfig);
            Console.WriteLine("default config created");
            return defaultConfig;
        }
        string json = File.ReadAllText(Program.GetFolderPath(config));
        return JsonSerializer.Deserialize<Config>(json);
    }

    public static void SaveConfig(Config cfg)
    {
        string json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Program.GetFolderPath(config), json);
    }
    
    public void EditConfig(string db)
    {
        Config config = LoadConfig();
        Console.Clear();
        Console.WriteLine($"enter database name [currently: {config.database}]:");
        string line = Console.ReadLine();
        while (!isValidName(line))
        {
            if (line.Equals(""))
            {
                Console.WriteLine("no name entered, using current");
                line = config.database;
                break;
            }
            Console.Clear();
            Console.WriteLine("database name may only contain alphanumeric characters, _, -, +, and .");
            Console.WriteLine($"re-enter database name [currently: {config.database}]:");
            line = Console.ReadLine();
        }
        config.database = line;
        
        SaveConfig(config);
        
        Console.WriteLine("\npress any key to return");
        Console.ReadKey(true);
        Program.Init();
    }
    
    bool isValidName(string name)
    {
        string pattern = @"^[a-zA-Z0-9_\-+.]+$";
        return Regex.IsMatch(name, pattern);
    }
}