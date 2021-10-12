using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCoffee.Models
{
    public class CartBuyView
    {
        //購物車內商品陣列
        public List<CartBuy> DataList { get; set; }
        //購物車是否已保存
        public bool isCartsave { get; set; }
    }
}