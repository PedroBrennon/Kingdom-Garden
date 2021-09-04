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
    public class ImageController : Controller
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



        public async Task<ActionResult> Index()
        {
            var id = (int)Session["galleryId"];
            var image = new GalleryViewModel { Id = id };

            var response = await _clientHelper.GetImages(image);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<List<ImageViewModel>>();

                return View(model);
            }

            return RedirectToAction("Index", "Gallery");
        }

        // GET: Image/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var response = await _clientHelper.GetImage(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<ImageViewModel>();

                return View(model);
            }

            return RedirectToAction("Index", "Gallery");
        }

        // GET: Image/Create
        public ActionResult Create()
        {
            var galleryId = (int)Session["galleryId"];
            var image = new ImageViewModel
            {
                GalleryId = galleryId
            };

            return View(image);
        }

        // POST: Image/Create
        [HttpPost]
        public async Task<ActionResult> Create(ImageViewModel image)
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

                image.Url = blob.Uri.ToString();

                var response = await _clientHelper.InsertImage(image);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Gallery");
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

        // GET: Image/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var response = await _clientHelper.GetImage(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<ImageViewModel>();

                return View(model);
            }

            return RedirectToAction("Index", "Gallery");
        }

        // POST: Image/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, ImageViewModel image)
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

                image.Url = blob.Uri.ToString();
                var galleryId = (int)Session["galleryId"];
                image.GalleryId = galleryId;

                var response = await _clientHelper.UpdateImage(id, image);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Gallery");
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

        // GET: Image/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _clientHelper.GetImage(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<ImageViewModel>();

                return View(model);
            }

            return RedirectToAction("Index", "Gallery");
        }

        // POST: Image/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, ImageViewModel image)
        {
            try
            {
                var response = await _clientHelper.DeleteImage(id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Gallery");
                }

                return View(image);
            }
            catch
            {
                return View();
            }
        }
    }
}
