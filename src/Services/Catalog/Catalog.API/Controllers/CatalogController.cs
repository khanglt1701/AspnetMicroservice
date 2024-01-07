using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            var products = await _productRepository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productRepository.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"Product with id {id}, not found");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("[action]/{category}", Name = "GetProductByCategory")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Product>> GetProductByCategory(string category)
        {
            var product = await _productRepository.GetProductByCategory(category);
            if (product == null)
            {
                _logger.LogError($"Product with category {category}, not found");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("[action]/{name}", Name = "GetProductByName")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Product>> GetProductByName(string name)
        {
            var product = await _productRepository.GetProductByCategory(name);
            if (product == null)
            {
                _logger.LogError($"Product with name {name}, not found");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _productRepository.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _productRepository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> DeleteProduct(string id)
        {
            return Ok(await _productRepository.DeleteProduct(id));
        }
    }
}
