using SimpleMinimalAPIs.Entities;

namespace SimpleMinimalAPIs.DumyService
{
    public class ProductService
    {
        private ICollection<Product> _products;
        public ProductService()
            => _products = ProductGenerator();


        public Product? GetById(Guid id)
        => _products.FirstOrDefault(p => p.Id.Equals(id));

        public bool Update(Guid id, Product product)
        {
            var foundedProduct = _products.FirstOrDefault(p => p.Id.Equals(id));

            if (foundedProduct is null)
                return false;

            foundedProduct.Price = product.Price;
            foundedProduct.Quantity = product.Quantity;
            foundedProduct.Title = product.Title;
            foundedProduct.Name = product.Name;

            return true;
        }

        public bool Delete(Guid id)
        {
            var foundedProduct = _products.FirstOrDefault(p => p.Id.Equals(id));

            if (foundedProduct is null)
                return false;

            _products.Remove(foundedProduct);
            return true;
        }

        public bool Add(Product product)
        {
            var foundedProduct = _products.FirstOrDefault(p => p.Id.Equals(product.Id));

            if (foundedProduct is not null)
                return false;

            _products.Add(product);
            return true;

        }
        public ICollection<Product> GetProducts()
            => _products;

        private ICollection<Product> ProductGenerator()
        => Enumerable.Range(0, 15).Select(x => new Product
        {
            Id = Guid.NewGuid(),
            Title = $"Product Title {x}",
            Name = $"Product{x}",
            Price = x * Random.Shared.Next(1, 100),
            Quantity = x * Random.Shared.Next(0, 500),
        }).ToList();





    }
}
