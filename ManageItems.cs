using System.Data.SQLite;

namespace inv_mng;

public class ManageItems
{
    public void Menu(string db)
    {
        Console.Clear();
        Console.WriteLine("manage inventory\n");
        Console.WriteLine("1. add item");
        Console.WriteLine("2. remove item"); //TODO: items stack, when item>1 ask how many remove
        Console.WriteLine("0. return\n");
        
        string input = Console.ReadLine();
        switch (input)
        {
            case "0":
                Program.Menu(db);
                break;
            case "1":
                AddItem(db);
                break;
            case "2":
                RemoveItem(db);
                break;
            default:
                Menu(db);
                break;
        }
    }

    void AddItem(string db)
    {
        Console.Clear(); //we should be able to optimize this somehow (make compact)
        Console.WriteLine("enter item name: ");
        string name = Console.ReadLine();
        if(name.Equals(""))
            name = "UNKNOWN";
        
        Console.Clear();
        Console.WriteLine("enter item description: ");
        string description = Console.ReadLine();
        if(description.Equals(""))
            description = "no description";
        
        int amount = parseAmount();
        
        Console.Clear();
        Console.WriteLine("enter item category: ");
        string category = Console.ReadLine();
        if(category.Equals(""))
            category = "other";
        
        string creationDate = DateTime.Today.ToString("yyyy-MM-dd");
        
        using (var connection = new SQLiteConnection($"Data Source={db};Version=3;"))
        { 
            connection.Open(); 
                
            string insertCharacter = @"
                INSERT INTO INVENTORY (name, description, amount, category, creationDate)
                VALUES (@name, @description, @amount, @category, @creationDate);";
            
            var command = new SQLiteCommand(insertCharacter, connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@category", category);
            command.Parameters.AddWithValue("@creationDate", creationDate);
            
            command.ExecuteNonQuery();
            Console.Clear();
            Console.WriteLine($"item '{name}' added (x{amount})\ndescription: '{description}'\ncategory: {category} | {creationDate}");
        }
        Console.WriteLine("\npress any key to return");
        Console.ReadKey(true);
        Program.Menu(db);
    }
    
    void RemoveItem(string db)
    {
        Console.Clear();
        Console.WriteLine("enter item id: ");
        string id = Console.ReadLine();
        while (id.Equals(""))
            id = Console.ReadLine();
        
        //int amount = parseAmount(); //TODO: add amount and check item amount: if greater than return error, if 0 delete item
        
        Console.Clear();
        if (int.TryParse(id, out int itemId))
        {
            using (var connection = new SQLiteConnection($"Data Source={db};Version=3;"))
            {
                connection.Open();
                
                string deleteItem = "DELETE FROM INVENTORY WHERE itemId = @itemId";
                using (var command = new SQLiteCommand(deleteItem, connection))
                {
                    command.Parameters.AddWithValue("@itemId", itemId);
                    int rowsAffected = command.ExecuteNonQuery();
                    if(rowsAffected > 0)
                        Console.WriteLine($"item '{id}' deleted (x{rowsAffected})");
                    else
                        Console.WriteLine($"item '{id}' not found");
                }
            }
        }
        else Console.WriteLine("invalid id");
        Console.WriteLine("\npress any key to return");
        Console.ReadKey(true);
        Program.Menu(db);
    }

    int parseAmount()
    {
        Console.Clear();
        Console.WriteLine("enter item amount: ");
        int amount;
        while (true)
        {
            string amountInput = Console.ReadLine();

            if (Int32.TryParse(amountInput, out amount) || amountInput.Equals("")) //optimize
            {
                if (amount == 0 || amountInput.Equals(""))
                    amount = 1;
                return amount;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("enter valid integer: ");
            }
        }
    }
}