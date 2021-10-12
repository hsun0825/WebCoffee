using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebCoffee.Models
{
    public class Mail
    {
        private string gmail_account = ""; //Gmail帳號
        private string gmail_password = ""; //Gmail密碼
        private string gmail_mail = ""; //Gmail信箱

        public string GetValidateCode()
        {
            //設定驗證碼字元的陣列
            string[] Code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K"
        , "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y"
            , "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b"
                , "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n"
                    , "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            //宣告初始為空的驗證碼字串
            string ValidateCode = string.Empty;
            //宣告可產生隨機數值的物件
            Random rd = new Random();
            //使用迴圈產生出驗證碼
            for(int i =0; i < 10; i++)
            {
                ValidateCode += Code[rd.Next(Code.Count())];
            }
            return ValidateCode;

        }
        #region 將使用者資料填入驗正信範本中
        
        public string GetRegisterMailBody(string TempString, String UserName,String ValidateUrl)
        {
            //將使用者資料填入
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);
            //回傳結果
            return TempString;
        }
        #endregion

        #region 寄驗證信的方法

        public void SendRegisterMail(string MailBoby,string ToEmail)
        {
            //建立寄信用stmp物件
            SmtpClient smtpSrever = new SmtpClient("smtp.gmail.com");
            //設定使用的Port，這裡設定Gmail所使用的
            smtpSrever.Port = 587;
            //建立使用者憑據，這裡要設定自己的Gmail帳戶
            smtpSrever.Credentials = new System.Net.NetworkCredential("marginallllly@gmail.com", "hsun0825");
            //開啟SSL
            smtpSrever.EnableSsl = true;
            //宣告信件內容物件
            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress("test@gmail.com");
            //設定收信者信箱
            mail.To.Add(ToEmail);
            //設定信件主旨
            mail.Subject = "會員註冊確認信";
            //設定信件內容
            mail.Body = MailBoby;
            //設定信件內容為HTML格式
            mail.IsBodyHtml = true;
            //送出信件
            smtpSrever.Send(mail);

        }
        #endregion

        #region 寄驗證信的方法

        public void SendPasswordMail(string MailBoby, string ToEmail)
        {
            //建立寄信用stmp物件
            SmtpClient smtpSrever = new SmtpClient("smtp.gmail.com");
            //設定使用的Port，這裡設定Gmail所使用的
            smtpSrever.Port = 587;
            //建立使用者憑據，這裡要設定自己的Gmail帳戶
            smtpSrever.Credentials = new System.Net.NetworkCredential("marginallllly@gmail.com", "hsun0825");
            //開啟SSL
            smtpSrever.EnableSsl = true;
            //宣告信件內容物件
            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress("test@gmail.com");
            //設定收信者信箱
            mail.To.Add(ToEmail);
            //設定信件主旨
            mail.Subject = "會員密碼";
            //設定信件內容
            mail.Body = MailBoby;
            //設定信件內容為HTML格式
            mail.IsBodyHtml = true;
            //送出信件
            smtpSrever.Send(mail);

        }
        #endregion





    }
}




