using Dapper;
using Discount.Grpc.Entities;
using Npgsql;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var connectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            using var connection = new NpgsqlConnection(connectionString);

            var result = await connection.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", coupon);
                /*new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });*/

            if (result == 0)
                return false;
            return true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var connectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            using var connection = new NpgsqlConnection(connectionString);

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName });

            if (coupon == null)
            {
                return new Coupon
                {
                    ProductName = "No Discount",
                    Amount = 0,
                    Description = "No Discount Desc"
                };
            }

            return coupon;

        }

        public async Task<bool> RemoveDiscount(string productName)
        {
            var connectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            using var connection = new NpgsqlConnection(connectionString);

            var result = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
            new { ProductName = productName });

            if (result == 0)
                return false;
            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var connectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            using var connection = new NpgsqlConnection(connectionString);

            var result = await connection.ExecuteAsync("UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id", coupon);
            /*new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });*/

            if (result == 0)
                return false;
            return true;
        }
    }
}
