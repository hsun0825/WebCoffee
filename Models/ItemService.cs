using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace WebCoffee.Models
{
    public class ItemService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["BabakWebEntities"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);


        #region 取得單一商品資料
        //藉由編號取的單筆商品資料的方法
        public Item GetDataById(int Id)
        {
            //回傳根據編號所取得的資料
            Item Data = new Item();

            //Sql語法Item
            string sql = $@" select * from Item where Id = {Id} ";
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
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Image = dr["Image"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Price = Convert.ToInt32(dr["Price"]);
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
            return Data;
        }
        #endregion

        #region 取得商品編號陣列
        public List<int> GetIdList(ForPaging Paging)
        {
            //計算所需的總頁面
            SetMaxPaging(Paging);
            //取得資料庫中的Item資料表
            List<int> IdList = new List<int>();
            //Sql語法
            //desc為反向排序 越晚新增的越前面
            string sql = $@" select Id from (select row_number() over(order by Id desc) as sort,* from Item ) m
Where m.sort Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum} ";
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
                    IdList.Add(Convert.ToInt32(dr["Id"]));
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
            //先排序再根據分頁來回傳所需的部分資料陣列
            return IdList;
        }
        #endregion

        #region 設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Item ";
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
                    Row++;
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
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }

        #endregion

        #region 新增商品
        //新增商品方法
        public void Insert(Item newData)
        {
            //取得最新一筆Id
            newData.Id = LastItemFinder();
            //Sql新增語法Item
            string sql = $@" INSERT INTO Item(Id,Name,Price,Image,GenreId) VALUES ( {newData.Id},'{newData.Name}',{newData.Price},'{newData.Image}',{newData.GenreId}) ";
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

        #region 計算目前商品最新一筆的Id
        public int LastItemFinder()
        {
            // 宣告要回傳的值
            int Id;

            //Sql查詢語法
            string sql = $@" select top 1 * from Item order by Id desc";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                // 取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["Id"]);
            }
            catch (Exception e)
            {
                //沒資料時Id為0
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id + 1;
        }
        #endregion


        #region 刪除商品
        //刪除商品方法
        public void Delete(int Id)
        {
            //Sql刪除語法CartBuy
            //根據商品編號刪除資料
            //先將CartBuy的資料刪除才刪Item
            //使用StringBuilder方法一次建立SQL做使用
            StringBuilder sql = new StringBuilder();
            sql.AppendLine($@" Delete from CartBuy where Item_Id = {Id} ");
            sql.AppendLine($@" Delete from Item where Id = {Id} ");
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                //使用StringBuilder需要再一次轉換成字串型態
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
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

        #region 下拉式選單 到Controller引用
        public List<Genre> droplist()
        {
            //回傳根據編號所取得的資料
            var GenreData = new List<Genre>();
            //Sql語法搜尋全部的Genre欄位的資料while撈出來
            string sql = $@" select * from Genre ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                //dr.Read(); //獲得下一筆資料直到沒有資料
                while (dr.Read())
                {
                    var Genrelist = new Genre();

                    Genrelist.GenreId = Convert.ToInt32(dr["GenreId"]);

                    Genrelist.Name = dr["Name"].ToString();

                    GenreData.Add(Genrelist);
                }


            }
            catch (Exception e)
            {
                //沒有資料傳回null
                GenreData = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            return GenreData;
        }
        #endregion


        #region 把商品一口氣撈出來
        public IEnumerable<Item> GetItemList(ForPaging Paging)
        {

            SetMaxPaging(Paging);

            string sqlgetItem = $@"select * from Item Where id Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum} ";



            return conn.Query<Item>(sqlgetItem, new { Paging });
        }
        #endregion

        #region 最大類別頁數
        public void MaxGenreid(ForPaging paging,int? genreid)
        {
            // 計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Item ";

            if (genreid != null)
            {
                sql += $"where genreid= { genreid }";
            }

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
                    Row++;
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
            //計算所需的總頁數
            paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            paging.SetRightPage();
        }
        #endregion






        #region 撈出商品依照類別

        public IEnumerable<Item> GetItemGenreId(int? genreid, ForPaging Paging)

        {
            MaxGenreid(Paging,genreid);

            string sql = $@"with q as(SELECT  ROW_NUMBER() OVER(ORDER BY [Id] ASC) AS RowNumber,[Id],[Name],[Price],[Image],[GenreId] FROM Item where GenreId = @genreid)
                            select * from q where RowNumber between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum}";

            return conn.Query<Item>(sql, new { genreid, Paging });
        }
        #endregion



       
    }





}
