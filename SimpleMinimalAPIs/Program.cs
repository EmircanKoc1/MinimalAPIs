using Microsoft.AspNetCore.Mvc;
using SimpleMinimalAPIs.DumyService;
using SimpleMinimalAPIs.Entities;
using System.Net;
using System.Text.Json;

namespace SimpleMinimalAPIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            //builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<ProductService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            var pattern = "api/product";

            app.MapGet(pattern, ([FromServices] ProductService _productService,
                [FromServices] ILogger<Product> _logger) =>
            {
                var products = _productService.GetProducts();
                _logger.LogInformation("products : {0}", JsonSerializer.Serialize(products));
                return products is null ? Results.NoContent() : Results.Ok(products);

            }).Produces<IEnumerable<Product>>(statusCode: (int)HttpStatusCode.OK)
            .Produces<IEnumerable<Product>?>(statusCode: (int)HttpStatusCode.NoContent);


            app.MapGet(pattern + "/{id:guid}", ([FromServices] ProductService _productService,
               [FromServices] ILogger<Product> _logger,
               [FromRoute] Guid id) =>
            {
                var product = _productService.GetById(id);
                _logger.LogInformation("products : {0}", JsonSerializer.Serialize(product));
                return product is null ? Results.NotFound("Product not found") : Results.Ok(product);
            }).Produces<Product>(statusCode: (int)HttpStatusCode.OK)
            .Produces<Product?>(statusCode: (int)HttpStatusCode.NotFound);



            app.MapPost(pattern, ([FromServices] ProductService _productService,
               [FromServices] ILogger<Product> _logger,
               [FromBody] Product product) =>
            {
                var isAdded = _productService.Add(product);

                if (!isAdded)
                    return Results.BadRequest("Product is exists");

                _logger.LogInformation("product added : {0}", JsonSerializer.Serialize(product));
                return Results.Ok("product added");

            }).Produces<string>(statusCode: (int)HttpStatusCode.OK)
            .Produces<string>(statusCode: (int)HttpStatusCode.BadRequest);

            app.MapDelete(pattern, ([FromServices] ProductService _productService,
              [FromServices] ILogger<Product> _logger,
              [FromQuery] Guid id) =>
            {
                var isDeleted = _productService.Delete(id);

                if (!isDeleted)
                    return Results.NotFound("Product not found");

                _logger.LogInformation("product deleted : {0}", id.ToString());
                return Results.Ok("Product deleted");

            }).Produces<string>(statusCode: (int)HttpStatusCode.NotFound)
            .Produces<string>(statusCode: (int)HttpStatusCode.OK);

            app.MapPut(pattern, ([FromServices] ProductService _productService,
                [FromServices] ILogger<Product> _logger,
                [FromQuery] Guid id,
                [FromBody] Product product) =>
            {
                var isUpdated = _productService.Update(id, product);

                if (!isUpdated)
                    return Results.BadRequest("Product not found");

                _logger.LogInformation("Product Updated {0}", id.ToString());
                return Results.Ok("Product updated");

            }).Produces<string>(statusCode: (int)HttpStatusCode.BadRequest)
            .Produces<string>(statusCode: (int)HttpStatusCode.OK);

            //app.MapControllers();

            app.Run();
        }
    }
}
