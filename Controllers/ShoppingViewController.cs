using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCoffee.Models;

namespace WebCoffee.Controllers
{
    public class ShoppingViewController : Controller
    {
        readonly ItemService itemService = new ItemService();
        // GET: ShoppingView
        public ActionResult Index(int page = 1)
        {
            var viewModel = new ViewGenerId();

            viewModel.Paging = new ForPaging(page);

            viewModel.Genres = itemService.droplist();

            viewModel.items = (List<Item>)itemService.GetItemList(viewModel.Paging);

            return View(viewModel);
        }

        //viewModel懶得寫一個值接寫在這裡
        public class ViewGenerId
        {
            public List<Genre> Genres { get; set; }
            public List<Item> items { get; set; }
            public Item image { get; set; }
            public int genreid { get; set; }

            public ForPaging Paging { get; set; }
        }

        //寫一個自製Browse url: "store/{genre}"
        public ActionResult Browse(int genreid, int page = 1)

        {

            var viewModel = new ViewGenerId();

            viewModel.Paging = new ForPaging(page);
            //var aa = (List<Item>)itemService.GetItemGenreId(genreid, viewModel.Paging);
            //var bb = aa.Where((x, index) => index > viewModel.Paging.ItemNum * page && index <= viewModel.Paging.ItemNum * (page + 1)).ToList();
            viewModel.items = (List<Item>)itemService.GetItemGenreId(genreid, viewModel.Paging);
            viewModel.genreid = genreid;
            return View("Index", viewModel);
        }




    }
}