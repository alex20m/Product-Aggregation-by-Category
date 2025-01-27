using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

class Program
{
    static async Task Main(string[] args)
    {
        const string apiUrl = "https://fakestoreapi.com/products"; // API endpoint to fetch product data
        const string outputFile = "grouped_products.json"; // File path to save the grouped products as JSON

        try
        {
            Console.WriteLine("Fetching product data..."); 
            var products = await FetchProducts(apiUrl); // Fetch products from the API

            Console.WriteLine("Grouping products by category...");
            var groupedProducts = GroupProductsByCategory(products); // Group products by their category

            Console.WriteLine("Writing grouped data to JSON file...");
            WriteToJsonFile(groupedProducts, outputFile); // Write grouped data to a JSON file

            Console.WriteLine($"Data successfully saved to {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}"); // Handle errors and display the message
        }

    }

    // Fetch products from the API and deserialize the response into a list of Product objects
    static async Task<List<Product>> FetchProducts(string apiUrl)
    {
        using var httpClient = new HttpClient(); // Create an HttpClient instance for making HTTP requests
        var response = await httpClient.GetStringAsync(apiUrl); // Fetch data from the API as a string
        return JsonSerializer.Deserialize<List<Product>>(response); // Deserialize the JSON response into a list of Product objects
    }

    // Group products by their category and return a dictionary
    static Dictionary<string, List<ProductDto>> GroupProductsByCategory(List<Product> products)
    {
        return products
            .GroupBy(p => p.category) // Group products by their category
            .ToDictionary(
                g => g.Key, // Use the category as the dictionary key
                g => g
                    .OrderBy(p => p.price) // Sort by price
                    .Select(p => new ProductDto { id = p.id, title = p.title, price = p.price }) // Map Product to ProductDto
                    .ToList() // Convert the result to a list
            );
    }

    static void WriteToJsonFile(Dictionary<string, List<ProductDto>> data, string filePath)
    {
        // Set options to write indented JSON and prevent escaping characters like apostrophes
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
        };
        var json = JsonSerializer.Serialize(data, options); // Serialize the data to a JSON string
        File.WriteAllText(filePath, json); // Write the JSON string to the specified file
    }

}

// Class representing a product fetched from the API
class Product
{
    public required int id { get; set; }
    public required string title { get; set; }
    public required decimal price { get; set; }
    public required string category { get; set; }
}

// Class representing a simplified product (DTO) used for grouping and saving
class ProductDto
{
    public required int id { get; set; }
    public required string title { get; set; }
    public required decimal price { get; set; }
}