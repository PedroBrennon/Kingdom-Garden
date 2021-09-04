using KingdomGarden.MVC.Helper;
using KingdomGarden.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using KingdomGarden.MVC.Attributes;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using System.IO;
using Microsoft.WindowsAzure.Storage;

namespace KingdomGarden.MVC.Controllers
{
    public class ProfileController : Controller
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

        // GET: Profile
        [Authentication]
        public ActionResult Index()
        {
            if(_tokenHelper.AccessToken == null)
            {
                return RedirectToAction("Login", "Account", null);
            }

            var id = (string)Session["userId"];

            return RedirectToAction("Profiles", "Profile");
        }

        // GET: Profile/Profiles/email
        [Authentication]
        public async Task<ActionResult> Profiles()
        {
            if (_tokenHelper.AccessToken == null)
            {
                return RedirectToAction("Login", "Account", null);
            }
            string userId = (string)Session["userId"];

            var response = await _clientHelper.GetProfile(userId);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<ProfileViewModel>();

                return View(model);
            }

            return View();
        }

        // GET: Profile/Create
        [Authentication]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profile/Create
        [Authentication]
        [HttpPost]
        public ActionResult Create(ProfileViewModel profile)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Profile/Edit/5
        [Authentication]
        public async Task<ActionResult> Edit()
        {
            var userId = (string)Session["userId"];

            var response = await _clientHelper.GetProfile(userId);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<ProfileViewModel>();

                return View(model);
            }

            return View();
        }

        // POST: Profile/Edit/5
        [HttpPost]
        [Authentication]
        public async Task<ActionResult> Edit(string id, ProfileViewModel profile)
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

                profile.ImageUrl = blob.Uri.ToString();

                var userId = (string)Session["userId"];

                var response = await _clientHelper.UpdateProfile(userId, profile);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                return View(profile);
            }
            catch
            {
                return View();
            }
        }

        // GET: Profile/Delete/5
        [Authentication]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Profile/Delete/5
        [HttpPost]
        [Authentication]
        public ActionResult Delete(int id, ProfileViewModel profile)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
