using System.Data.SQLite;

namespace inv_mng;

class Items
{
    static Inventory inventory = new Inventory();

    public void AddItem(string db)
    {
        string id;
        string name;
        string description;
        int amount;
        string category;

        Console.WriteLine("\nenter item id (leave empty for new item): ");
        id = Console.ReadLine();
        if (id.Equals(""))
        {
            Console.WriteLine("\nenter item name: ");
            name = Console.ReadLine();
            if (name.Equals(""))
                name = "UNKNOWN";

            Console.WriteLine("\nenter item description: ");
            description = Console.ReadLine();
            if (description.Equals(""))
                description = "no description";

            amount = ParseAmount();

            Console.WriteLine("\nenter item category: ");
            category = Console.ReadLine();
            if (category.Equals(""))
                category = "other";

            string creationDate = DateTime.Today.ToString("yyyy-MM-dd");
            using (var connection = new SQLiteConnection($"Data Source={db};Version=3;"))
            {
                connection.Open();
                AddNewItem(connection, name, description, amount, category, creationDate);
            }
        }
        else
        {
            amount = ParseAmount();

            using (var connection = new SQLiteConnection($"Data Source={db};Version=3;"))
            {
                connection.Open();
                AddItemAmount(connection, id, amount);
            }
        }

        Console.WriteLine("\npress any key to return");
        Console.ReadKey(true);
        Program.Init();
    }

    void AddItemAmount(SQLiteConnection connection, string itemId, int amount)
    {
        try
        {
            string updateInventory = "UPDATE INVENTORY SET amount = amount + @amount WHERE itemId = @itemId;";
            using (var command = new SQLiteCommand(updateInventory, connection))
            {
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@itemId", itemId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Console.WriteLine("\nadded amount");
                else
                    Console.WriteLine("\nfailed to add amount");
            }
        }
        catch (SQLiteException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    void AddNewItem(SQLiteConnection connection, string name, string description, int amount, string category,
        string creationDate)
    {
        string insertInventory = @"
            INSERT INTO INVENTORY (name, description, amount, category, creationDate)
            VALUES (@name, @description, @amount, @category, @creationDate);
         ";

        var command = new SQLiteCommand(insertInventory, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@category", category);
        command.Parameters.AddWithValue("@creationDate", creationDate);

        command.ExecuteNonQuery();
        Console.WriteLine(
            $"item '{name}' added (x{amount})\ndescription: '{description}'\ncategory: {category} | {creationDate}");
    }

    ///
    /// 
    ///
    
    public void RemoveItem(string db)
    {
        Console.Clear();
        inventory.ViewInventory(db);

        Console.WriteLine("\nenter item id: ");
        string id = Console.ReadLine();
        while (id.Equals(""))
            id = Console.ReadLine();

        int amount = ParseAmount();
        int itemAmount = GetAmount(db, id);
        while (amount > itemAmount)
        {
            Console.WriteLine($"amount cant be higher than item amount [{amount}:{itemAmount}]");
            amount = ParseAmount();
        }

        while (amount <= 0)
        {
            Console.WriteLine($"amount cant be lower than or equal to zero [{amount}]");
            amount = ParseAmount();
        }

        if ((amount - itemAmount) == 0)
        {
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
                        if (rowsAffected > 0)
                            Console.WriteLine($"item '{id}' deleted (x{amount}:({itemAmount}))");
                        else
                            Console.WriteLine($"item '{id}' not found");
                    }
                }
            }
            else Console.WriteLine("invalid id");
        }
        else
        {
            if (int.TryParse(id, out int itemId))
            {
                using (var connection = new SQLiteConnection($"Data Source={db};Version=3;"))
                {
                    connection.Open();

                    string deleteItem = "UPDATE INVENTORY SET amount = amount - @amount WHERE itemId = @itemId;";
                    using (var command = new SQLiteCommand(deleteItem, connection))
                    {
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@itemId", itemId);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            Console.WriteLine($"item '{id}' deleted (x{amount}:({itemAmount}))");
                        else
                            Console.WriteLine($"item '{id}' not found");
                    }
                }
            }
            else Console.WriteLine("invalid id");
        }

        Console.WriteLine("\npress any key to return");
        Console.ReadKey(true);
        Program.Init();
    }
    
    ///
    ///
    ///

    int GetAmount(string db, string itemId)
    {
        using (var connection = new SQLiteConnection($"Data Source={db};Version=3;"))
        {
            try
            {
                string getItem = "SELECT amount FROM INVENTORY WHERE itemId = @itemId LIMIT 1;";
                using (var command = new SQLiteCommand(getItem, connection))
                {
                    command.Parameters.AddWithValue("@itemId", itemId);
                    var result = command.ExecuteScalar();

                    if (result != null)
                        return Convert.ToInt32(result);
                    else
                        return -1;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }

    int ParseAmount()
    {
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