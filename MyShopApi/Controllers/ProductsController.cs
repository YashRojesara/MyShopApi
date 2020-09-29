using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MyShopApi.Models;
using Newtonsoft.Json.Linq;

namespace MyShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;    

        private readonly MyShopContext _context;

        public ProductsController(MyShopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _hostingEnvironment = environment;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(x => x.Catgory).ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("Get/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            var category = _context.Categories.FirstOrDefault(x => x.CategoryId == product.CatgoryId);
            product.Catgory = category;
            product.CatgoryId = category?.CategoryId;

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("Put/{id}")]
        public async Task<ActionResult<Product>> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await GetProduct(product.ProductId);
        }

        [HttpPost("AddInvoice")]
        public async Task<ActionResult> Addnvoice([FromBody] JObject data)
        {
            Customer customer = data["customerData"].ToObject<Customer>();
            List<Product> products = data["productData"].ToObject<List<Product>>();

            if (customer != null && products != null)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                var invoiceList = new List<Invoice>();
                foreach (var product in products)
                {
                    var invoice = new Invoice
                    {
                        CustomerId = customer.CustomerId,
                        ProductId = product.ProductId,
                        Qty = product.Qty,
                        Price = (product.Price + (product.Price * product.Gst / 100) -
                                    (product.Price * product.Discount / 100)) * product.Qty,
                    };
                    invoiceList.Add(invoice);
                }
                _context.Invoices.AddRange(invoiceList);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        [HttpPut("ModifyProductQty")]
        public async Task<IActionResult> ModifyProductQty(List<Product> products)
        {
            if (products != null)
            {
                foreach (var product in products)
                {
                    var prod = _context.Products.FirstOrDefault(x => x.ProductId == product.ProductId);
                    if (prod != null)
                    {
                        prod.Qty = product.Qty;
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                return NoContent();
            }
            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("Post")]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            product.Image = Path.GetFileName(product.Image);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetProduct(product.ProductId);
            //return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        /// <summary>
        /// Used to upload images in Image folder
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        public ActionResult Upload(IFormFile files)
        {
            try
            {
                var fileModel = new FileModel
                {
                    FileName = files?.FileName,
                    FormFile = files
                };

                using FileStream filstream = System.IO.File.Create(_hostingEnvironment.WebRootPath + $@"\{fileModel.FileName}");
                fileModel?.FormFile.CopyTo(filstream);
                filstream.Flush();

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        // DELETE: api/Products/5
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
