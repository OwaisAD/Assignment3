using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assignment3;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        var listener = new TcpListener(IPAddress.Loopback, 5001);
        listener.Start();
        Console.WriteLine("Server listening on {0}", listener.LocalEndpoint);

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Task.Run(() => HandleClient(client));
        }
    }

    private static void HandleClient(TcpClient client)
    {
        try
        {
            using var stream = client.GetStream();
            var buffer = new byte[2048];
            var bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead <= 0) return;

            var requestJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Try deserialize
            Request? request = null;
            try
            {
                request = JsonSerializer.Deserialize<Request>(requestJson);
            }
            catch
            {
                // ignored
            }

            var validator = new RequestValidator();
            var categoryService = new CategoryService();
            var response = new Response();

            // Invalid or empty request
            if (request == null)
            {
                response.Status = "4 missing date";
                SendResponse(stream, response);
                return;
            }

            // Validate
            response = validator.ValidateRequest(request);
            if (!response.Status.StartsWith("1"))
            {
                SendResponse(stream, response);
                return;
            }

            var parser = new UrlParser();
            if (!parser.ParseUrl(request.Path))
            {
                response.Status = "4 Bad Request";
                SendResponse(stream, response);
                return;
            }

            // Route by method
            switch (request.Method.ToLower())
            {
                case "echo":
                    response.Status = "1 Ok";
                    response.Body = request.Body;
                    break;

                case "read":
                    if (parser.Path != "/api/categories")
                    {
                        response.Status = "5 Not found";
                        break;
                    }

                    if (parser.HasId)
                    {
                        var cat = categoryService.GetCategory(parser.Id);
                        if (cat == null)
                        {
                            response.Status = "5 Not found";
                        }
                        else
                        {
                            response.Status = "1 Ok";
                            response.Body = JsonSerializer.Serialize(cat,
                                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                        }
                    }
                    else
                    {
                        response.Status = "1 Ok";
                        response.Body = JsonSerializer.Serialize(categoryService.GetCategories(),
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    }

                    break;

                case "create":
                    if (parser.HasId)
                    {
                        response.Status = "4 Bad Request";
                        break;
                    }

                    var obj = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);
                    var name = obj["name"];
                    var newId = categoryService.GetCategories().Max(c => c.Id) + 1;
                    categoryService.CreateCategory(newId, name);
                    response.Status = "2 Created";
                    response.Body = JsonSerializer.Serialize(categoryService.GetCategory(newId),
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    break;

                case "update":
                    if (!parser.HasId)
                    {
                        response.Status = "4 Bad Request";
                        break;
                    }

                    var upd = JsonSerializer.Deserialize<Category>(request.Body);
                    var ok = categoryService.UpdateCategory(parser.Id, upd.Name);
                    response.Status = ok ? "3 Updated" : "5 Not found";
                    break;

                case "delete":
                    if (!parser.HasId)
                    {
                        response.Status = "4 Bad Request";
                        break;
                    }

                    var deleted = categoryService.DeleteCategory(parser.Id);
                    response.Status = deleted ? "1 Ok" : "5 Not found";
                    break;

                default:
                    response.Status = "4 Bad Request";
                    break;
            }

            SendResponse(stream, response);
        }
        catch
        {
            // z
        }
        finally
        {
            client.Close();
        }
    }

    private static void SendResponse(NetworkStream stream, Response response)
    {
        var json = JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var bytes = Encoding.UTF8.GetBytes(json);
        stream.Write(bytes, 0, bytes.Length);
    }
}