using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class Item // Class for itme details
{
    public string Name { get; set; } // Item naem
    public decimal ShelfPrice { get; set; } // Price per item
    public bool IsImported { get; set; } // Impoted status
    public bool IsExempt { get; set; } // Tax exemtion status
    public int Quantity { get; set; } // Qunatity of item

    public Item(string name, decimal shelfPrice, bool isImported, bool isExempt, int quantity) // Constructor with itme details
    {
        Name = name;
        ShelfPrice = shelfPrice;
        IsImported = isImported;
        IsExempt = isExempt;
        Quantity = quantity;
    }
}

public class Receipt // Receipt cass
{
    private List<Item> items = new List<Item>(); // List to hold itmes
    private decimal salesTaxes = 0; // Total sales txes
    private decimal total = 0; // Total cost including taxes

    public void AddItem(Item item) // Method to add itmes
    {
        // Consolidate items if they already exist, checking by name, price and imported stauts
        var existingItem = items.FirstOrDefault(i => i.Name == item.Name && i.ShelfPrice == item.ShelfPrice && i.IsImported == item.IsImported);
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity; // Increase qunatity
        }
        else
        {
            items.Add(item); // Add new item to list
        }
    }

    private decimal RoundUpTax(decimal amount) // Method to round up taxes
    {
        return Math.Ceiling(amount * 20) / 20; // Rounds up to nearest 0.05
    }

    public void CalculateTotals() // Calculate totals for the receipt
    {
        foreach (var item in items)
        {
            decimal taxRate = 0;
            if (!item.IsExempt) taxRate += 0.10m; // Add basic sales tax
            if (item.IsImported) taxRate += 0.05m; // Add import duty

            decimal tax = RoundUpTax(item.ShelfPrice * taxRate) * item.Quantity;
            decimal taxedPrice = item.ShelfPrice + RoundUpTax(item.ShelfPrice * taxRate);
            decimal totalPrice = taxedPrice * item.Quantity;

            salesTaxes += tax; // Add to total txes
            total += totalPrice; // Add to total cost

            if (item.Quantity > 1)
                Console.WriteLine($"{item.Name}: {totalPrice.ToString("0.00", CultureInfo.InvariantCulture)} ({item.Quantity} @ {taxedPrice.ToString("0.00", CultureInfo.InvariantCulture)})");
            else
                Console.WriteLine($"{item.Name}: {taxedPrice.ToString("0.00", CultureInfo.InvariantCulture)}");
        }

        Console.WriteLine($"Sales Taxes: {salesTaxes.ToString("0.00", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Total: {total.ToString("0.00", CultureInfo.InvariantCulture)}");
    }
}

public class Program
{
    public static void Main() // Main mthod
    {
        Receipt receipt = new Receipt(); // Creating a receipt object
        Console.WriteLine("Enter all item details, each on a new line, in the format 'quantity item name at price'.");
        Console.WriteLine("Type 'done' when finished to calculate the receipt:");

        string input;
        while ((input = Console.ReadLine()) != "done")
        {
            if (string.IsNullOrWhiteSpace(input)) continue;

            var details = input.Split(" at ");
            if (details.Length != 2)
            {
                Console.WriteLine("Invalid input format. Please use 'quantity item name at price'");
                continue;
            }

            string[] parts = details[0].Split(new[] { ' ' }, 2);
            if (parts.Length != 2)
            {
                Console.WriteLine("Please enter the correct format: 'quantity item name at price'");
                continue;
            }

            int quantity = int.Parse(parts[0]);
            string name = parts[1];
            decimal price = decimal.Parse(details[1]);

            bool isImported = name.ToLower().Contains("imported");
            bool isExempt = name.ToLower().Contains("book") || name.ToLower().Contains("chocolate") || name.ToLower().Contains("pill");

            receipt.AddItem(new Item(name, price, isImported, isExempt, quantity));
        }

        receipt.CalculateTotals();
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}