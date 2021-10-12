using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCoffee.Models;

namespace WebCoffee.Controllers
{
    [Checklogin]
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCart shopping = new ShoppingCart();




        //填寫收件人資料進入結帳最後流程
        public ActionResult Index()
        {
           
            return View();
        }

        [HttpPost]
        public ActionResult Index(WebCoffee.Models.Order order)
        {
            if (!ModelState.IsValid)

                return View();
            //資料儲存
           
            shopping.AddOrder(order, User.Identity.Name);


            return RedirectToAction("Index","Home");
        }

        public ActionResult Edit()
       
        {
          
         if(shopping.CheckOrderId(User.Identity.Name) == false)
            {
                return Content("購物車沒資料");
            }

            return View(shopping.Read(User.Identity.Name));
        }

        //存進去購物車
        public ActionResult AddInCart(int Item_id)
        {
            shopping.AddToCart(User.Identity.Name, Item_id);

            return RedirectToAction("Edit");

        }

        public ActionResult UpdateProductCount(int id, int count)
        {
            shopping.UpdateProductCount(User.Identity.Name, id, count);

            return Json(new { Status = 200, Message = "執行成功" });
           // return Json(new { id, count });
        }


        ////再見了我的更新價錢不需要你了
        //public bool UpdateProductPrice(int Id,int count)
        //{
        //    shopping.UpdateProductPrice(Id,count);

        //    return true;

        //}

        #region 訂單查看
        public ActionResult  OrderList()
        {

            return View(shopping.ReadOrderList(User.Identity.Name));
        }
        #endregion


        #region 訂單明細

        public ActionResult OrderListDetial(string orderid)
        {
            

            return View(shopping.ReadOrderdetailList(orderid));
        }

        #endregion

        


    }


}