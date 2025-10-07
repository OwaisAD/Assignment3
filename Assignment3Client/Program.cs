using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Assignment3Client;

internal abstract class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            using var client = new TcpClient("localhost", 5001);
            await using var stream = client.GetStream();
            Console.WriteLine("Connected to server on localhost:5001");
            Console.WriteLine("Type JSON requests as a single line. Type 'exit' to quit.\n");

            while (true)
            {
                Console.Write("Enter JSON request: ");
                var input = Console.ReadLine();
                if (input == null || input.ToLower() == "exit") break;

                // Ensure input is valid JSON
                try
                {
                    using var doc = JsonDocument.Parse(input); // just to validate
                }
                catch
                {
                    Console.WriteLine("Invalid JSON. Try again.");
                    continue;
                }

                var bytes = Encoding.UTF8.GetBytes(input + "\n"); // newline marks end
                await stream.WriteAsync(bytes, 0, bytes.Length);

                // Read response
                var buffer = new byte[4096];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                var responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Server response:");
                Console.WriteLine(responseJson);
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}