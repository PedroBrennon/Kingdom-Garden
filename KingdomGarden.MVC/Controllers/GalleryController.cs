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
    public class GalleryController : Controller
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
            var id = (string)Session["userId"];

            var response = await _clientHelper.GetGalleries(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<List<GalleryViewModel>>();

                return View(model);
            }

            return RedirectToAction("Index", "Profile");
        }

        // GET: Gallery/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var response = await _clientHelper.GetGallery(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<GalleryViewModel>();
                Session["galleryId"] = model.Id;

                return View(model);
            }

            return RedirectToAction("Index");
        }

        // GET: Gallery/Create
        public ActionResult Create()
        {
            var id = (string)Session["userId"];
            var gallery = new GalleryViewModel { UserId = id };
            
            return View(gallery);
        }

        // POST: Gallery/Create
        [HttpPost]
        public async Task<ActionResult> Create(GalleryViewModel gallery)
        {
            try
            {
                var response = await _clientHelper.InsertGallery(gallery);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Gallery/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var response = await _clientHelper.GetGallery(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<GalleryViewModel>();

                return View(model);
            }

            return RedirectToAction("Index");
        }

        // POST: Gallery/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, GalleryViewModel gallery)
        {
            try
            {
                var response = await _clientHelper.UpdateGallery(id, gallery);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                return View(gallery);
            }
            catch
            {
                return View();
            }
        }

        // GET: Gallery/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _clientHelper.GetGallery(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<GalleryViewModel>();

                return View(model);
            }

            return RedirectToAction("Index");
        }

        // POST: Gallery/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, GalleryViewModel gallery)
        {
            try
            {
                var response = await _clientHelper.DeleteGallery(id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Gallery");
                }

                return View(gallery);
            }
            catch
            {
                return View();
            }
        }
    }
}
