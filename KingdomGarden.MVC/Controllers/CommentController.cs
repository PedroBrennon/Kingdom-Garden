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
    public class CommentController : Controller
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



        /*public ActionResult Index()
        {
            return View();
        }*/

        // GET: Comment/Details/5
        [Authentication]
        public async Task<ActionResult> Details(int id)
        {
            var response = await _clientHelper.GetComment(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<CommentViewModel>();

                return View(model);
            }

            return RedirectToAction("Index");
        }

        // GET: Comment/Create
        [Authentication]
        public ActionResult Create(int postId)
        {
            var userId = (string)Session["userId"];
            var comment = new CommentViewModel { PostId = postId, UserId = userId };

            return View(comment);
        }

        // POST: Comment/Create
        [HttpPost]
        [Authentication]
        public async Task<ActionResult> Create(CommentViewModel model)
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

                var response = await _clientHelper.InsertComment(model);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Post");
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Comment/Edit/5
        [Authentication]
        public async Task<ActionResult> Edit(int id)
        {
            var model = new CommentViewModel();
            var response = await _clientHelper.GetComment(id);

            if (response.IsSuccessStatusCode)
            {
                model = await response.Content.ReadAsAsync<CommentViewModel>();
            }

            return View(model);
        }

        // POST: Comment/Edit/5
        [HttpPost]
        [Authentication]
        public async Task<ActionResult> Edit(int id, CommentViewModel model)
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

                var response = await _clientHelper.UpdateComment(id, model);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Post");
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Comment/Delete/5
        [Authentication]
        public async Task<ActionResult> Delete(int id)
        {
            var model = new CommentViewModel();
            var response = await _clientHelper.GetComment(id);

            if (response.IsSuccessStatusCode)
            {
                model = await response.Content.ReadAsAsync<CommentViewModel>();
            }

            return View(model);
        }

        // POST: Comment/Delete/5
        [HttpPost]
        [Authentication]
        public async Task<ActionResult> Delete(int id, CommentViewModel comment)
        {
            try
            {
                var response = await _clientHelper.DeleteComment(id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Post");
                }

                return View(comment);
            }
            catch
            {
                return View();
            }
        }
    }
}
