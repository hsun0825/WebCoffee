using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebCoffee.Models
{
    public class ItemCreate
    {
        [DisplayName("商品圖片")]
        [FileExtensions(ErrorMessage = "所上傳檔案不是圖片")]
        //HttpPostedFileBase 為檔案上傳的操作類別
        public HttpPostedFileBase ItemImage { get; set; }
        //新增商品內容
        public Item NewData { get; set; }

        //讀取的值只要GenreId得值
        public int GenreId { get; set; }
   
    }

}