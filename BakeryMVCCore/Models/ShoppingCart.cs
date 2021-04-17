using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BakeryMVCCore.Models
{
    public class ShoppingCart
    {
        private readonly BakeryDBContext _ctx;
        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        private ShoppingCart(BakeryDBContext ctx)
        {
            _ctx = ctx;
        }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<BakeryDBContext>();

            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Pie pie, int amount = 1)
        {
            var shoppingCartItem = _ctx.ShoppingCartItems.SingleOrDefault(s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pie = pie,
                    Amount = 0
                };

                _ctx.ShoppingCartItems.Add(shoppingCartItem);
            }

            shoppingCartItem.Amount += amount;
            _ctx.SaveChanges();
        }

        public int RemoveFromCart(Pie pie, int amount = 1)
        {
            var shoppingCartItem = _ctx.ShoppingCartItems.SingleOrDefault(s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);

            var localAmount = 0;
            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount <= 1)
                    _ctx.ShoppingCartItems.Remove(shoppingCartItem);

                shoppingCartItem.Amount -= amount;
                localAmount = shoppingCartItem.Amount;
            }

            _ctx.SaveChanges();
            return localAmount;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            var result = ShoppingCartItems ?? (ShoppingCartItems =
                _ctx.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId)
                .Include(s => s.Pie).ToList());

            return result;
        }

        public void ClearCart()
        {
            var cartItems = _ctx.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId);

            _ctx.RemoveRange(cartItems);
            _ctx.SaveChanges();
        }

        public decimal GetShoppingCartTotal()
        {
            var total = _ctx.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId)
                .Select(c => c.Pie.Price * c.Amount).Sum();

            return total;
        }
    }
}
