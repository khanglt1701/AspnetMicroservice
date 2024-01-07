using Catalog.API.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {

        public CatalogContext(IConfiguration configuration, IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            Products = mongoDatabase.GetCollection<Product>(databaseSettings.Value.CollectionName);
            CatalogContextSeed.SeedData(Products);
        }
        public IMongoCollection<Product> Products { get; }
    }
}
