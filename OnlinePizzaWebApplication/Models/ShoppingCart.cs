using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlinePizzaWebApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace OnlinePizzaWebApplication.Models
{
    public class ShoppingCart
    {
        private readonly AppDbContext _appDbContext;

        private ShoppingCart(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public string ShoppingCartId { get; set; }

        public List<ShoppingCartItem> ShoppingCartItems { get; set; }


        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            var context = services.GetService<AppDbContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }
        
        public async Task AddToCartAsyncBoxes(Pizzas pizza, int amountB, int amountS)
        {
            var shoppingCartItem =
                    await _appDbContext.ShoppingCartItems.SingleOrDefaultAsync(
                        s => s.Pizza.Id == pizza.Id && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pizza = pizza,
                    AmountBoxes = amountB,
                    AmountSingles= amountS
                };

                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.AmountBoxes++;
            }

            await _appDbContext.SaveChangesAsync();
        }
        public async Task AddToCartAsyncSingles(Pizzas pizza, int amountB, int amountS)
        {
            var shoppingCartItem =
                    await _appDbContext.ShoppingCartItems.SingleOrDefaultAsync(
                        s => s.Pizza.Id == pizza.Id && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pizza = pizza,
                    AmountBoxes = amountB,
                    AmountSingles = amountS
                };

                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.AmountSingles++;
            }

            await _appDbContext.SaveChangesAsync();
        }
        public async Task UpdateToCartAsyncSingles(Pizzas pizza, int amountS)
        {
            var shoppingCartItem =
                    await _appDbContext.ShoppingCartItems.SingleOrDefaultAsync(
                        s => s.Pizza.Id == pizza.Id && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pizza = pizza,
                    AmountSingles = amountS
                };

                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.AmountSingles=amountS;
            }

            await _appDbContext.SaveChangesAsync();
        }
        public async Task UpdatePrice(Pizzas pizza, int price)
        {
            var shoppingCartItem =
                    await _appDbContext.ShoppingCartItems.SingleOrDefaultAsync(
                        s => s.Pizza.Id == pizza.Id && s.ShoppingCartId == ShoppingCartId);

                shoppingCartItem.Pizza.Price = price;
            

            await _appDbContext.SaveChangesAsync();
        }


        public async Task UpdateToCartAsyncBoxes(Pizzas pizza, int amountB)
        {
            var shoppingCartItem =
                    await _appDbContext.ShoppingCartItems.SingleOrDefaultAsync(
                        s => s.Pizza.Id == pizza.Id && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pizza = pizza,
                    AmountBoxes = amountB,
                };

                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.AmountBoxes = amountB;
            }

            await _appDbContext.SaveChangesAsync();
        }
        public async Task AddToCartAsync(Pizzas pizza, int amountB, int amountS)
        {
            var shoppingCartItem =
                    await _appDbContext.ShoppingCartItems.SingleOrDefaultAsync(
                        s => s.Pizza.Id == pizza.Id && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pizza = pizza,
                    AmountBoxes = amountB,
                    AmountSingles = amountS
                };

                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.AmountBoxes++;
            }

            await _appDbContext.SaveChangesAsync();
        }
        public async Task<int> RemoveFromCartAsync(Pizzas pizza)
        {
            var shoppingCartItem =
                    await _appDbContext.ShoppingCartItems.SingleOrDefaultAsync(
                        s => s.Pizza.Id == pizza.Id && s.ShoppingCartId == ShoppingCartId);

            var localAmount = 0;

            if (shoppingCartItem != null)
            {
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);   
            }

            await _appDbContext.SaveChangesAsync();

            return localAmount;
        }

        public async Task<int> UpdateCartAsync(Pizzas pizza, int amountB, int amountS)//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            var shoppingCartItem =
                    await _appDbContext.ShoppingCartItems.SingleOrDefaultAsync(
                        s => s.Pizza.Id == pizza.Id && s.ShoppingCartId == ShoppingCartId);
            var localAmount = 0;//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pizza = pizza,
                    AmountBoxes = amountB,
                    AmountSingles = amountS
                };

                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                if (shoppingCartItem.AmountBoxes > 1)
                {
                    shoppingCartItem.AmountBoxes--;
                    localAmount = shoppingCartItem.AmountBoxes;
                }
                else
                {
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }

            await _appDbContext.SaveChangesAsync();

            return localAmount;
        }
    


















        public async Task<List<ShoppingCartItem>> GetShoppingCartItemsAsync()
        {
            return ShoppingCartItems ??
                   (ShoppingCartItems = await
                       _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                           .Include(s => s.Pizza)
                           .ToListAsync());
        }

        public async Task ClearCartAsync()
        {
            var cartItems = _appDbContext
                .ShoppingCartItems
                .Where(cart => cart.ShoppingCartId == ShoppingCartId);

            _appDbContext.ShoppingCartItems.RemoveRange(cartItems);

            await _appDbContext.SaveChangesAsync();
        }

        public float GetShoppingCartTotal()
        {
            var total = _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                .Select(c => c.Pizza.Price * c.AmountBoxes).Sum();
            return total;
        }

    }
}
