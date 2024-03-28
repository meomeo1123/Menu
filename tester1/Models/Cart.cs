using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tester1.Models
{
    public class CartItem
    {
        public SanPham product { get; set; }
        public int Quantity { get; set; }
        public SalePromotionDTO SalePromotionDTO { get; set; }
    }
    
    public class Cart
    {
        List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }
        public void Update_Quantity_Shopping(int id, int _quantity)
        {
            var item = items.Find(s => s.product.MaSP == id);
            if (item != null)
            {
                item.Quantity = _quantity;
            }
        }
        public double Total_Money()
        {
            var total = items.Sum(s => s.product.GiaBan * s.Quantity);
            return (double)total;
        }
        // tổng số lượng mua sắm
        public int Total_Quantity_in_Cart()
        {
            return items.Sum(s => s.Quantity);
        }
    

    }
}