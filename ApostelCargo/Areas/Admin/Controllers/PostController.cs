using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApostelCargo.Data;
using ApostelCargo.Models;
using ApostelCargo.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApostelCargo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public PostController(ApplicationDbContext db,
                              UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    post.UserId = user.Id;
                    post.Picture = p1;
                    post.DateCreated = DateTime.UtcNow;
                }
                _db.Post.Add(post);
                await _db.SaveChangesAsync();
                return Redirect("/Customer/Home/Blog");
            }
            return Redirect("/Customer/Home/Blog");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Redirect("/Admin/Account/ErrorSomethingWentWrong");
            }

            var post = await _db.Post.FindAsync(id);

            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            return PartialView(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post)
        {
            var user = await _userManager.GetUserAsync(User);
            var postFromDb = await _db.Post.Where(p => p.PostId == post.PostId).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    postFromDb.Picture = p1;
                    postFromDb.UserId = user.Id;
                }

                postFromDb.Description = post.Description;


                await _db.SaveChangesAsync();
                return Redirect("/Customer/Home/Blog");
            }
            return Redirect("/Customer/Home/Blog");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var post = await _db.Post.FindAsync(id);

            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            return PartialView(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            var service = await _db.Post.SingleOrDefaultAsync(s => s.PostId == post.PostId);
            var commentCount = await _db.CommentCount.SingleOrDefaultAsync(c => c.PostId == post.PostId);
            var comment = await _db.Comments.Where(c => c.PostId == post.PostId).ToListAsync();
            var likesCount = await _db.LikeCount.SingleOrDefaultAsync(l => l.PostId == post.PostId);

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            _db.Post.Remove(service);

            foreach (var item in comment)
            {
                _db.Comments.Remove(item);
            }
            
            _db.CommentCount.Remove(commentCount);
            _db.LikeCount.Remove(likesCount);

            await _db.SaveChangesAsync();
            return Redirect("/Customer/Home/Blog");

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<JsonResult> PostComment(Comments model)
        //{
        //    bool success = false;

        //    if(ModelState.IsValid)
        //    {
        //        model.DateCreated = DateTime.UtcNow;
        //        _db.Comments.Add(model);
        //        await _db.SaveChangesAsync();
        //        success = true;
        //        return new JsonResult(Comments, JsonRequestBehavior.AllowGet);
        //    }

        //    return new JsonResult(success);
        //}

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] Comments model)
        {
            var UserId = "";
            var Email = "";
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                UserId = "0";
                Email = "Anonymous";
            }
            else
            {
                UserId = user.Id;
                Email = user.Email;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    CommentCount CoummentCountFromDb = await _db.CommentCount.Where(c => c.PostId == model.PostId).FirstOrDefaultAsync();

                    if(CoummentCountFromDb == null)
                    {
                        var commentCountModel = new CommentCount
                        {
                            UserId = UserId,
                            PostId = model.PostId
                        };

                        _db.CommentCount.Add(commentCountModel);
                    }
                    else
                    {
                        CoummentCountFromDb.Count = CoummentCountFromDb.Count + 1;
                    }

                    model.DateCreated = DateTime.UtcNow;
                    model.UserId = UserId;
                    model.Email = Email;
                    _db.Comments.Add(model);
                    await _db.SaveChangesAsync();

                }
                return Json(new { status = "ok" });
            }
            catch (Exception e)
            {

                return Json(new { status = e.Message });
            }
        }

        public async Task<IActionResult> AddLike([FromBody] Comments model)
        {
            var UserId = "";
            var Email = "";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                UserId = "0";
                Email = "Anonymous";
            }
            else
            {
                UserId = user.Id;
                Email = user.Email;
            }

            try
            {
                if(ModelState.IsValid)
                {
                    LikeCount LikeCountFromDb = await _db.LikeCount.Where(l => l.PostId == model.PostId).FirstOrDefaultAsync();
                    if(LikeCountFromDb == null)
                    {
                        var LikeCountModel = new LikeCount()
                        {
                            UserId = UserId,
                            PostId = model.PostId
                        };
                        _db.LikeCount.Add(LikeCountModel);
                    }
                    else
                    {
                        LikeCountFromDb.Count = LikeCountFromDb.Count + 1;
                    }
                    await _db.SaveChangesAsync();
                }
                return Json(new { status = "ok" });
            }
            catch(Exception e)
            {
                return Json(new { status = e.Message });
            }
        }

        //public async Task<IActionResult> GetComments(int postId)
        //{
        //    //var postId = Convert.ToInt32(postIds);
        //    List<Comments> comments = new List<Comments>();

        //    //comments = await (from c in _db.Comments
        //    //                  where c.PostId == postId
        //    //                  select c).ToListAsync();
        //    comments = await _db.Comments.Where(c => c.PostId == postId).ToListAsync();
        //    //return Json(new { commentsval = comments });
        //    return Json(comments);
        //}


        public async Task<IActionResult> GetComments(int Id)
        {
            List<Comments> comments = new List<Comments>();

            comments = await _db.Comments.Where(p => p.PostId == Id).ToListAsync();
            return PartialView(comments);
            //}
        }

        public async Task<IActionResult> GetCommentAndLikesCount(int Id)
        {
            List<Comments> comments = new List<Comments>();

            CommentsAndLikesViewModels CommentsAndLikesVM = new CommentsAndLikesViewModels()
            {
                Comments = await _db.Comments.Where(p => p.PostId == Id).ToListAsync()
            };
            return PartialView(CommentsAndLikesVM);
            //}
        }

        public async Task<IActionResult> GetCommentsCount(int Id)
        {
            List<Comments> comments = new List<Comments>();

            comments = await _db.Comments.Where(p => p.PostId == Id).ToListAsync();
            return Json(comments.Count());
            //}
        }
    }
}