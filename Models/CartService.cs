using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebCoffee.Models
{
    public class CartService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["BabakWebEntities"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        #region 取得購物車內商品陣列
        //取得於購物車內的商品陣列
        public List<CartBuy> GetItemFromCart(string Cart)
        {
            //宣告要回傳的搜尋資料為資料庫中的CartBuy資料表
            List<CartBuy> DataList = new List<CartBuy>();
            //Sql語法
            //根據購物車編號取得已放入購物車的商品陣列
            string sql = $@" select * from CartBuy m inner join Item d on m.Item_Id = d.Id  where Cart_Id = '{Cart}' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    CartBuy Data = new CartBuy();
                    Data.Cart_Id = dr["Cart_Id"].ToString();
                    Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                    Data.Item.Id = Convert.ToInt32(dr["Id"]);
                    Data.Item.Image = dr["Image"].ToString();
                    Data.Item.Name = dr["Name"].ToString();
                    Data.Item.Price = Convert.ToInt32(dr["Price"]);
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;

        }
        #endregion

        #region 確認商品是否於購物車中
        //確認商品是否於購物車中方法
        public bool CheckInCart(string Cart, int Item_Id)
        {
            //宣告要回傳的搜尋資料為資料庫中的CartBuy資料
            CartBuy Data = new CartBuy();
            //Sql語法
            //根據購物車與商品編號取得CartBuy資料表內資料
            string sql = $@" select * from CartBuy m inner join Item d on m.Item_Id = d.Id where Cart_Id = '{Cart}' and Item_Id = {Item_Id} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read(); //獲得下一筆資料直到沒有資料
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                Data.Item.Id = Convert.ToInt32(dr["Id"]);
                Data.Item.Image = dr["Image"].ToString();
                Data.Item.Name = dr["Name"].ToString();
                Data.Item.Price = Convert.ToInt32(dr["Price"]);
            }
            catch (Exception e)
            {
                //沒有資料傳回null
                Data = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //判斷是否有資料，以確認是否於購物車中
            return (Data != null);
        }
        #endregion

        #region 放入購物車
        //將商品放入購物車方法
        public void AddtoCart(string Cart, int Item_Id)
        {
            //Sql新增語法CartBuy
            string sql = $@" INSERT INTO CartBuy(Cart_Id,Item_Id) VALUES ( '{Cart}',{Item_Id} ) ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 從購物車取出
        //將商品從購物車取出方法
        public void RemoveForCart(string Cart, int Item_Id)
        {
            //Sql刪除語法CartBuy
            //根據購物車與商品編號取得要刪除的資料
            string sql = $@" Delete from CartBuy where Cart_Id = '{Cart}' and Item_Id = {Item_Id} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 確認購物車是否有保存
        //確認商品是否於購物車中方法
        public bool CheckCartSave(string Account, string Cart)
        {
            //根據會員帳號與購物車編號取得CartSave資料表內資料
            CartSave Data = new CartSave();
            //Sql語法
            string sql = $@" select * from CartSave m inner join Members d on m.Account = d.Account where m.Account = '{Account}' and Cart_Id = '{Cart}' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read(); //獲得下一筆資料直到沒有資料
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Members.Name = dr["Name"].ToString();
            }
            catch (Exception e)
            {
                //沒有資料傳回null
                Data = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //判斷是否有資料，以確認是否於購物車中
            return (Data != null);
        }
        #endregion

        #region 取得購物車保存
        //取得購物車保存方法
        public string GetCartSave(string Account)
        {
            //根據會員帳號與購物車編號取得CartSave資料表內資料
            CartSave Data = new CartSave();
            //Sql語法CartSave
            string sql = $@" select * from CartSave m inner join Members d on m.Account = d.Account where m.Account = '{Account}' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {

                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read(); //獲得下一筆資料直到沒有資料
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Members.Name = dr["Name"].ToString();
            }
            catch (Exception e)
            {
                //沒有資料傳回null
                Data = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //判斷是否有資料，以確認是否於購物車中
            if (Data != null)
            {
                return Data.Cart_Id;
            }
            else
            {
                return null;
            }
        }
        #endregion  

        #region 保存購物車
        public void SaveCart(string Account, string Cart)
        {
            //Sql新增語法CartSave
            string sql = $@" INSERT INTO CartSave(Account,Cart_Id) VALUES ( '{Account}','{Cart}' ) ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 取消保存購物車
        public void SaveCartRemove(string Account)
        {
            //Sql刪除語法CartSave
            //根據會員帳號取得要刪除的資料
            string sql = $@" Delete from CartSave where Account = '{Account}' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion
    }
}