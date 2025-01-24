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
            WriteToJsonFile(groupedProducts, outputFilePath);

            Console.WriteLine($"Data successfully saved to {outputFilePath}");
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
    

}

class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
}