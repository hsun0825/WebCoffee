using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WebCoffee.Models;
using WebCoffee.Security;

namespace WebCoffee.Controllers
{
    public class MembersController : Controller
    {
        // GET: Member
        //宣告Members資料表的Service物件
        private readonly MemberDB membersService = new MemberDB();
        //宣告寄信用的Service物件
        private readonly Mail mailService = new Mail();
        //宣告Cart相關的Service物件
        private readonly CartService cartService = new CartService();





        #region 註冊
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            //已登入則重新導向
            //否則進入註冊畫面
            return View();
        }
        [HttpPost]
        public ActionResult Register(MemberRegisterView memberRegister)
        {
            if (ModelState.IsValid)
            {
                //將頁面資料中的密碼欄位填入
                memberRegister.newMember.Password = memberRegister.Password;
                //取得信箱驗證碼
                string AuthCode = mailService.GetValidateCode();
                //將信箱驗證碼填入
                memberRegister.newMember.AuthCode = AuthCode;
                //呼叫Serrvice註冊新會員
                membersService.Register(memberRegister.newMember);

                //取得寫好的驗證信範本內容
                string TempMail = System.IO.File.ReadAllText(
                    Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));

                //宣告Email驗證用的Url
                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("EmailValidate", "Members"
                    , new
                    {
                        Account = memberRegister.newMember.Account,
                        AuthCode = AuthCode
                    })
                };
                //藉由Service將使用者資料填入驗證信範本中
                string MailBody = mailService.GetRegisterMailBody(TempMail, memberRegister.newMember.Name, ValidateUrl.ToString().Replace("%3F", "?"));//URL中的特殊字元是不能再URL中直接傳遞的，需要進行編碼。編碼的格式為：%加字元的ASCII碼，即一個百分號%，後面跟對應字元的ASCII（16進位制）碼值。
                //呼叫Service寄出驗證信
                mailService.SendRegisterMail(MailBody, memberRegister.newMember.Email);
                //用TempData儲存註冊訊息
                TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                //重新導向頁面
                return RedirectToAction("RegisterResult");



            }
            //未經驗證清空密碼相關欄位
            memberRegister.Password = null;
            memberRegister.PasswordCheck = null;
            return View(memberRegister);
        }

        #endregion

        #region 註冊結果
        //註冊結果顯示頁面
        public ActionResult RegisterResult()
        {
            return View();
        }

        //判斷註冊帳號是否已被註冊過Action
        public JsonResult AccountCheck(MemberRegisterView memberRegister)
        {
            //呼叫Service來判斷，並回傳結果
            return Json(membersService.AccountCheck(memberRegister.newMember.Account),
                JsonRequestBehavior.AllowGet);
        }

        //接收驗證信連結傳進來的Action
        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            //用ViewData儲存，使用Service進行信箱驗證後的結果訊息
            ViewData["EmailValidate"] = membersService.EmailValidate(Account, AuthCode);
            return View();
        }

        #endregion


        #region 登入
        [AllowAnonymous]
        public ActionResult Login()
        {
            //判斷使用者是否已經過登入驗正
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        //傳入登入資料的Action
        [HttpPost] //設定此Action只接受頁面POST資料傳入
        public ActionResult Login(MerberLonginView longinMember)
        {
            //使用Service裡的方法來驗證登入的帳號密碼
            string ValidateStr = membersService.LoginCheck(longinMember.Account, longinMember.Password);
            //判斷驗證後結果是否有錯誤訊息
            if (String.IsNullOrEmpty(ValidateStr))
            {
                //無錯誤訊息，則登入
                //先清空Session
                HttpContext.Session.Clear();

                //取得購物車保存
                string Cart = cartService.GetCartSave(longinMember.Account);
                //判斷是否有保存，若有則存入Session
                if (Cart != null)
                {
                    HttpContext.Session["Cart"] = Cart;
                }

                //先藉由Service取得登入者角色資料
                string RoleData = membersService.GetRole(longinMember.Account);
                //設定JWT
                JwtService jwtService = new JwtService();
                //從Web.Config撈出資料
                //Cookie名稱
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                string Token = jwtService.GenerateToken(longinMember.Account, RoleData);
                ////產生一個Cookie
                HttpCookie cookie = new HttpCookie(cookieName);
                //設定單值
                cookie.Value = Server.UrlEncode(Token);
                //寫到用戶端
                Response.Cookies.Add(cookie);
                //設定Cookie期限
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                //重新導向頁面
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //有驗證錯誤訊息，加入頁面模型中
                ModelState.AddModelError("", ValidateStr);
                //將資料回填至View中
                return View(longinMember);
            }


        }
        #endregion

        #region 登出
        //登出Action
        [Authorize] //設定此Action須登入
        public ActionResult Logout()
        {
            //使用者登出
            //Cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            //清除Cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            //重新導向至登入Action
            return RedirectToAction("Login");
        }
        #endregion


        #region 修改密碼
        //修改密碼一開始載入頁面
        [Authorize] //設定此Action須登入
        public ActionResult ChangePassword()
        {
            return View();
        }
        //修改密碼傳入資料Action
        [Authorize] //設定此Action須登入
        [HttpPost] //設定此Action接受頁面POST資料傳入
        public ActionResult ChangePassword(MemberChangePasswordView ChangeData)
        {
            //判斷頁面資料是否都經過驗證
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }
            return View();
        }
        #endregion

        #region 忘記密碼
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            // forgotPasswordViewModel.Email > Email
            membersService.ForgotPass(forgotPasswordViewModel);

            var mailbody = "您的新密碼是 11 請修改客戶密碼";
            mailService.SendPasswordMail(mailbody, forgotPasswordViewModel.Email);

            return View(forgotPasswordViewModel);
         
        }
        #endregion

        public ActionResult ForgotPassword()
        {
            return View();
        }







        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

    


    }



}


    