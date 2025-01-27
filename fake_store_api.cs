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
        const string apiUrl = "https://fakestoreapi.com/products";
        const string outputFile = "grouped_products.json";

        try
        {
            Console.WriteLine("Fetching product data...");
            var products = await FetchProducts(apiUrl);

            Console.WriteLine("Grouping products by category...");
            var groupedProducts = GroupProductsByCategory(products);

            Console.WriteLine("Writing grouped data to JSON file...");
            WriteToJsonFile(groupedProducts, outputFile);

            Console.WriteLine($"Data successfully saved to {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

    }

    static async Task<List<Product>> FetchProducts(string apiUrl)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(apiUrl);
        return JsonSerializer.Deserialize<List<Product>>(response);
    }

    static Dictionary<string, List<ProductDto>> GroupProductsByCategory(List<Product> products)
    {
        return products
            .GroupBy(p => p.category)
            .ToDictionary(
                g => g.Key,
                g => g
                    .OrderBy(p => p.price) // Sort by price
                    .Select(p => new ProductDto { id = p.id, title = p.title, price = p.price })
                    .ToList()
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
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }

}

class Product
{
    public required int id { get; set; }
    public required string title { get; set; }
    public required decimal price { get; set; }
    public required string category { get; set; }
}

class ProductDto
{
    public required int id { get; set; }
    public required string title { get; set; }
    public required decimal price { get; set; }
}