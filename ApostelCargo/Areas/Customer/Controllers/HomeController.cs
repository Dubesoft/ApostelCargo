using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApostelCargo.Models;
using ApostelCargo.Data;
using Microsoft.EntityFrameworkCore;
using ApostelCargo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace ApostelCargo.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Blog()
        {
            

            BlogViewModel blogVM = new BlogViewModel()
            {
                Post = await _db.Post.ToListAsync(),
                //CommentCount = await _db.CommentCount.ToListAsync()
            };
            var postList = await _db.Post.ToListAsync();
            for (int i = 0; i < postList.Count(); i++)
            {
                var commentCount = await _db.CommentCount.Where(c =>c.PostId == postList[i].PostId).ToListAsync();
                var likeCount = await _db.LikeCount.Where(l => l.PostId == postList[i].PostId).ToListAsync();
                foreach (var item in commentCount)
                {
                    HttpContext.Session.SetInt32("cmtCount" + i, item.Count);
                }

                foreach (var item in likeCount)
                {
                    HttpContext.Session.SetInt32("likesCount" + i, item.Count);
                }

            }
            return View(blogVM);
            //return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
