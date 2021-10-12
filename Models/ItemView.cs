using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCoffee.Models
{
    public class ItemView
    {
        //顯示資料陣列
        public List<int> IdList { get; set; }
        //商品區塊
        public List<ItemDetail> ItemBlock { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }
    }
}