using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Microsoft.Ajax.Utilities;

namespace WebCoffee.Models
{
    public class ShoppingCart
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["BabakWebEntities"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);



        //string datenum = DateTime.Now.ToString("yyyyMMdd") + "000001";

        public void AddToCart(string Account, int Item_id)

        {
            string sql = "SELECT * FROM [Cart] WHERE Account = @Account and Id = @Item_id and OrderId is Null";

            //建立Cart的model

            //計算金額list
            //var CC = conn.Query<Int32>("select Item.Price*Cart.count from Item join Cart on Item.Id = Cart.Id"); 


            //建立cart model
            var cart = conn.QueryFirstOrDefault<Cart>(sql, new { Account, Item_id });



            //var pricelist = new List<Cart>();

            if (cart == null)
            {
                // insert 
                //CC foreach回圈list
                //CC.ForEach(price => pricelist.Add(new Cart()
                //{
                //    Count = 1,
                //    Account = Account,
                //    Id = Item_id,
                //    DateCreated = DateTime.Now,
                //    Price = price
                //}));

                cart = new Cart()
                {
                    Count = 1,
                    Account = Account,
                    Id = Item_id,
                    DateCreated = DateTime.Now

                };
                string test = @"insert Cart ([Count],[Id],[Account],[DateCreated],[Price]) 
                                select @Count,@Id,@Account,@DateCreated,price 
                                 from Item where Id =@Id";

                conn.Execute(test, cart);
            }
            else
            {
                cart.Count++;
                cart.DateCreated = DateTime.Now;

                // update 
                var updateSql = "UPDATE [Cart] SET [Count] = @Count, [DateCreated] = @DateCreated  WHERE [Account] = @Account and [Id]=@Id ";
                //var updateSql = "UPDARE [Cart] [Count] = @Count, [DateCreated] = @DateCreated WHERE [Account] = '' OR 1 ";
                conn.Execute(updateSql, cart);

            }

            // var sql = $@" INSERT INTO Cart(RecordId,CartId,DateCreated,Count,Id) VALUES ( '{datenum}','{Account}','{DateTime.Now}','{1}',{Item_id})";


        }

        public void UpdateProductCount(string account, int id, int count)

        {


                if (count != 0)
                {
                    string Updatesqlcount = "update [Cart] set [Count] = @count where [Account]=@account and [Id]=@id";
                    conn.Execute(Updatesqlcount, new { count, account, id });
                }
                else
                {
                    string deletesql = "delete from Cart Where [Account]=@account and [Id]=@id";
                    conn.Execute(deletesql, new { account, id });
                }



          


            //string Updatesqlcount = "update [Cart] set [Count] = @count where [Account]=@account and [Id]=@id";
            //conn.Execute(Updatesqlcount, new { count, account, id });
        }
        public IEnumerable<Cart> Read(string Account)
        {
            //string sql = "SELECT * FROM [Cart] WHERE Account = @Account";
            string sql = "select  Cart.Account,Cart.Count,Cart.DateCreated,Cart.RecordId,Cart.Id,Cart.Price,Item.Name,Item.Image " +
                "from [BabakWeb].[dbo].[Cart] " +
                "inner join [BabakWeb].[dbo].[Item] " +
                "on Cart.Id=Item.Id  " +
                "WHERE Account = @Account and OrderId is Null";


            var carts = conn.Query<Cart>(sql, new { Account });
            //cart中再找到item裡面的image
            carts.ForEach((i) =>
            {
                sql = "SELECT * from item where Id=@Id";
                i.Item = new Item();
                i.Item.Image = conn.Query<Item>(sql, new { Id = i.Id}).FirstOrDefault().Image;
            }
            );
            return carts;
        }

        public void AddOrder(WebCoffee.Models.Order order, string accountName)
        {

            var OrderId = Guid.NewGuid().ToString();

            //var _order = new Order()
            //{
            //    OrderId = OrderId,
            //    FristName = order.FristName,
            //    LastName = order.LastName,
            //    Email = order.Email,
            //    Phone = order.Phone,
            //    Address = order.Address,
            //    PostalCode = order.PostalCode,
            //    OrderDate = order.OrderDate,
            //    Account = Account,
            //};
            order.OrderId = OrderId;
            order.Account = accountName;
            order.OrderDate = DateTime.Now;

            conn.Open();
            string orderAdd = "INSERT [Order] (OrderId,FristName,LastName,Email,Phone,Address,PostalCode,OrderDate,Account) VALUES (@OrderId,@FristName,@LastName,@Email,@Phone,@Address,@PostalCode,@OrderDate,@Account)";
            conn.Execute(orderAdd, order);

            string UpdateOrderId = "update Cart set OrderId =@OrderId where Account=@Account and OrderId is Null";
            conn.Execute(UpdateOrderId, order);
        }

        //瀏覽Order資料畫面
        public IEnumerable<Order> ReadOrderList(string Account)
        {
            string readsql = "select * from [BabakWeb].[dbo].[Order] where Account = @Account";

            return conn.Query<Order>(readsql, new { Account });
        }

        //確認訂單結帳沒
        public bool CheckOrderId(string Account)
        {
            string checkSql = "select * from cart where Account =@Account and OrderId is null";

            return conn.ExecuteScalar<bool>(checkSql, new { Account });
        }
        //訂單細項
        public IEnumerable<Cart> ReadOrderdetailList(string Orderid)
        {
            string checksql = "  select * from Cart inner join Item on Item.Id=Cart.Id ";

            return conn.Query<Cart>(checksql, new { Orderid });
        }




        ////再見了我得funtion
        //public void UpdateProductPrice(int Id ,int count)
        //{
        //    try
        //    {
        //        if(count != 0)
        //        {


        //            string Updatesqlprice = "update Cart set price = Item.Price * Cart.Count from Cart inner join Item on Cart.Id = Item.Id where Cart.Id = @Id";


        //            conn.Execute(Updatesqlprice, new { Id, count });
        //        }


        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}




    }
}