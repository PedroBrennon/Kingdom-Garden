using KingdomGarden.MVC.Attributes;
using KingdomGarden.MVC.Helper;
using KingdomGarden.MVC.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KingdomGarden.MVC.Controllers
{
    public class PostController : Controller
    {
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;
        private const string _blobContainerName = "pedro-paiva";
        private ApiHelper _clientHelper = new ApiHelper();
        private TokenHelper _tokenHelper = new TokenHelper();

        public async Task SetupCloudBlob()
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            _blobClient = storageAccount.CreateCloudBlobClient();
            _blobContainer = _blobClient.GetContainerReference(_blobContainerName);

            await _blobContainer.CreateIfNotExistsAsync();
            var permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await _blobContainer.SetPermissionsAsync(permissions);
        }

        public string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }



        //


        [Authentication]
        public async Task<ActionResult> Index()
        {
            if (_tokenHelper.AccessToken == null)
            {
                return RedirectToAction("Login", "Account", null);
            }
            var id = (string)Session["userId"];
            var profile = new ProfileViewModel { Email = id };

            var response = await _clientHelper.GetPostByFriends(profile);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<List<PostViewModel>>();

                return View(model);
            }

            return RedirectToAction("Index", "Profile");
        }

        [Authentication]
        public async Task<ActionResult> MyPosts()
        {
            var id = (string)Session["userId"];

            var response = await _clientHelper.GetPostByUserId(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<List<PostViewModel>>();

                return View(model);
            }

            return RedirectToAction("Index", "Profile");
        }

        [Authentication]
        public async Task<ActionResult> Details(int id)
        {
            var response = await _clientHelper.GetPostById(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<PostViewModel>();
                Session["postId"] = model.Id;

                return View(model);
            }

            return RedirectToAction("Index", "Profile");
        }

        [Authentication]
        public ActionResult Create()
        {
            var userId = (string)Session["userId"];

            var post = new PostViewModel { UserId = userId };

            return View(post);
        }
        
        [HttpPost]
        [Authentication]
        public async Task<ActionResult> Create(PostViewModel model)
        {
            try
            {
                if (model.ImageUrl != null)
                {
                    HttpFileCollectionBase files = Request.Files;
                    int fileCount = files.Count;

                    if (fileCount == 0)
                    {
                        return View();
                    }

                    await SetupCloudBlob();
                    var fileName = GetRandomBlobName(files[0].FileName);
                    var blob = _blobContainer.GetBlockBlobReference(fileName);
                    await blob.UploadFromStreamAsync(files[0].InputStream);

                    model.ImageUrl = blob.Uri.ToString();
                }

                var response = await _clientHelper.InsertPost(model);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        [Authentication]
        public async Task<ActionResult> Edit(int id)
        {
            var model = new PostViewModel();
            var response = await _clientHelper.GetPostById(id);

            if(response.IsSuccessStatusCode)
            {
                model = await response.Content.ReadAsAsync<PostViewModel>();
            }

            return View(model);
        }
        
        [HttpPost]
        [Authentication]
        public async Task<ActionResult> Edit(int id, PostViewModel post)
        {
            try
            {
                HttpFileCollectionBase files = Request.Files;
                int fileCount = files.Count;

                if (fileCount == 0)
                {
                    return View();
                }

                await SetupCloudBlob();
                var fileName = GetRandomBlobName(files[0].FileName);
                var blob = _blobContainer.GetBlockBlobReference(fileName);
                await blob.UploadFromStreamAsync(files[0].InputStream);

                post.ImageUrl = blob.Uri.ToString();

                var response = await _clientHelper.UpdatePost(id, post);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Profile");
                }

                return View(post);
            }
            catch
            {
                return View();
            }
        }

        [Authentication]
        public async Task<ActionResult> Delete(int id)
        {
            var model = new PostViewModel();
            var response = await _clientHelper.GetPostById(id);

            if (response.IsSuccessStatusCode)
            {
                model = await response.Content.ReadAsAsync<PostViewModel>();
            }

            return View(model);
        }
        
        [HttpPost]
        [Authentication]
        public async Task<ActionResult> Delete(int id, PostViewModel post)
        {
            try
            {
                var response = await _clientHelper.DeletePost(id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Profile");
                }

                return View(post);
            }
            catch
            {
                return View();
            }
        }

        [Authentication]
        public async Task<ActionResult> DeleteLike(int id)
        {
            var like = new LikeViewModel();
            var userId = (string)Session["userId"];
            var response = await _clientHelper.GetLikeByPostId(id);

            if(response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<List<LikeViewModel>>();

                foreach (var l in model)
                {
                    if(l.PostId == id && l.UserId == userId)
                    {
                        like = new LikeViewModel
                        {
                            Id = l.Id,
                            UserId = userId,
                            PostId = id
                        };
                    }
                }
            }

            return View(like);
        }

        [HttpPost]
        [Authentication]
        public async Task<ActionResult> DeleteLike(int Id, LikeViewModel model)
        {
            try
            {
                var userId = (string)Session["userId"];
                var response = await _clientHelper.DeleteLike(Id, userId);

                return RedirectToAction("Index", "Post");
            }
            catch
            {
                return View();
            }
        }

        [Authentication]
        public async Task<ActionResult> InsertLike(int postId)
        {
            var userId = (string)Session["userId"];
            var responseLike = await _clientHelper.GetLikeByPostId(postId);
            var like = new LikeViewModel();

            if (responseLike.IsSuccessStatusCode)
            {
                var modelLike = await responseLike.Content.ReadAsAsync<IEnumerable<LikeViewModel>>();

                foreach (var l in modelLike)
                {
                    if (l.UserId == userId)
                    {
                        ViewBag.Like = "You already gave Like!";
                        return View("ErrorLike");
                    }
                }
            }

            like = new LikeViewModel { PostId = postId, UserId = userId };

            return View(like);
        }

        [HttpPost]
        [Authentication]
        public async Task<ActionResult> InsertLike(LikeViewModel like)
        {
            try
            {
                var response = await _clientHelper.InsertLike(like);

                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
