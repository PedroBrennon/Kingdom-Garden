using KingdomGarden.MVC.Helper;
using KingdomGarden.MVC.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KingdomGarden.MVC.Controllers
{
    public class AccountController : Controller
    {
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;
        private const string _blobContainerName = "pedro-paiva";
        private ApiHelper _clientHelper = new ApiHelper();
        private TokenHelper _tokenHelper;

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

        public AccountController()
        {
            _tokenHelper = new TokenHelper();
        }
        // GET: Account
        public async Task<ActionResult> Index()
        {
            var response = await _clientHelper.GetProfiles();
            var count = 0;

            if(response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadAsStringAsync();

                count = Convert.ToInt32(model);
            }

            ViewData["count"] = count;
            return View();
        }

        //GET Account/Register
        public ActionResult Register()
        {
            return View();
        }
        //POST Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
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
                var response = await _clientHelper.Register(model);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    return View();
                }
            }
            return View();
        }
        //GET Account/Login
        public ActionResult Login()
        {
            return View();
        }
        //POST Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var data = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", loginViewModel.Email },
                    { "password", loginViewModel.Password }
                };

                using (var requestContent = new FormUrlEncodedContent(data))
                {
                    var response = await _clientHelper.Login(requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenData = JObject.Parse(responseContent);

                        _tokenHelper.AccessToken = tokenData["access_token"];

                        ViewBag.Login = "Login efetuado com sucesso!";
                        Session["userId"] = loginViewModel.Email;

                        return RedirectToAction("Profiles", "Profile");
                    }
                    else
                    {

                    }
                }
;
            }
            return View(loginViewModel);
        }
        //GET Account/Delete
        public ActionResult Delete()
        {
            return View();
        }
        //POST Account/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var data = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", loginViewModel.Email },
                    { "password", loginViewModel.Password }
                };

                using (var requestContent = new FormUrlEncodedContent(data))
                {
                    var response = await _clientHelper.Login(requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenData = JObject.Parse(responseContent);

                        _tokenHelper.AccessToken = tokenData["access_token"];

                        ViewBag.Message = "Login efetuado com sucesso!";
                        var idProfile = "/" + loginViewModel.Email + "/";
                        //return View("Profile/Profiles", loginViewModel.Email);
                        return RedirectToAction("Profiles", "Profile", new { id = idProfile });
                    }
                    else
                    {

                    }
                }
;
            }
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout()
        {
            var data = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", "email" },
                    { "password", "password" }
                };
            var content = new FormUrlEncodedContent(data);
            var response = await _clientHelper.Logout(content);
            Session.Abandon();
            Session.Clear();

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

    }
}
