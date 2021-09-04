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
    public class FriendsController : Controller
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

            var response = await _clientHelper.GetFriends(id);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<List<FriendViewModel>>();

                return View(model);
            }

            return RedirectToAction("Index", "Profile");
        }

        public async Task<ActionResult> OtherUsers()
        {
            var id = (string)Session["userId"];
            var friends = new List<FriendViewModel>();
            var p = new ProfileViewModel { Email = id };

            var response = await _clientHelper.GetAllProfilesByUserId(p);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<List<ProfileViewModel>>();
                var userId = model.SingleOrDefault(x=> x.Email == id);

                model.Remove(userId);

                return View(model);
            }

            return RedirectToAction("Index", "Profile");
        }

        // GET: Friends/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var model = new FriendViewModel();
            var response = await _clientHelper.GetFriendsById(id);

            if (response.IsSuccessStatusCode)
            {
                model = await response.Content.ReadAsAsync<FriendViewModel>();
            }

            return View(model);
        }

        // GET: Friends/Create
        public ActionResult Create(string id)
        {
            var userId = (string)Session["userId"];

            var friend = new FriendViewModel { UserId1 = userId, UserId2 = id };

            return View(friend);
        }

        // POST: Friends/Create
        [HttpPost]
        public async Task<ActionResult> Create(FriendViewModel model)
        {
            try
            {
                var response = await _clientHelper.InsertFriend(model);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Profile");
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> DetailsFriend(string friend)
        {
            var response = await _clientHelper.GetProfile(friend);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsAsync<ProfileViewModel>();

                return View(model);
            }

            return View();
        }

        /* GET: Friends/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View();
        }

        // POST: Friends/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }*/

        public async Task<ActionResult> Delete(int id)
        {
            var model = new FriendViewModel();
            var response = await _clientHelper.GetFriendsById(id);

            if (response.IsSuccessStatusCode)
            {
                model = await response.Content.ReadAsAsync<FriendViewModel>();
            }

            return View(model);
        }
        
        [HttpPost]
        public async Task<ActionResult> Delete(int id, FriendViewModel model)
        {
            try
            {
                var response = await _clientHelper.DeleteFriend(id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Profile");
                }

                return View(model);
            }
            catch
            {
                return View();
            }
        }
    }
}
