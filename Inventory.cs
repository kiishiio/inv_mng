using System.Data.SQLite;

namespace inv_mng;

public class Inventory
{
    public void ViewInventory(string db)
    {
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
            //TODO: when view function, give option for add/remove function
        }
    }
}