using KingdomGarden.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace KingdomGarden.MVC.Helper
{
    public class ApiHelper
    {
        private readonly HttpClient _client;

        public ApiHelper()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://kingdomgardenapi.azurewebsites.net//")
            };
            _client.DefaultRequestHeaders.Accept.Clear();

            var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(mediaType);
        }

        #region Account
        public async Task<HttpResponseMessage> Register(RegisterViewModel model)
        {
            return await _client.PostAsJsonAsync("api/Account/Register", model);
        }

        public async Task<HttpResponseMessage> Login(FormUrlEncodedContent requestContent)
        {
            return await _client.PostAsync("Token", requestContent);
        }
        public async Task<HttpResponseMessage> Logout(FormUrlEncodedContent content)
        {
            return await _client.PostAsync("api/Account/Logout", content);
        }
        #endregion

        #region Profile
        public async Task<HttpResponseMessage> GetProfiles()
        {
            return await _client.GetAsync("api/Profile?count=0");
        }

        public async Task<HttpResponseMessage> GetAllProfilesByUserId(ProfileViewModel profile)
        {
            return await _client.GetAsync($"api/Profile?userId={profile.Email}");
        }

        public async Task<HttpResponseMessage> GetProfile(string id)
        {
            return await _client.GetAsync($"api/Profile//{id}/");
        }

        public async Task<HttpResponseMessage> UpdateProfile(string Email, ProfileViewModel model)
        {
            return await _client.PutAsJsonAsync($"api/Profile//{Email}/", model);
        }

        public async Task<HttpResponseMessage> InsertProfile(ProfileViewModel model)
        {
            return await _client.PostAsJsonAsync("api/Profile", model);
        }

        public async Task<HttpResponseMessage> DeleteProfile(int id)
        {
            return await _client.DeleteAsync($"api/Profile/{id}");
        }
        #endregion

        #region Post
        public async Task<HttpResponseMessage> GetPostById(int id)
        {
            return await _client.GetAsync($"api/Post/{id}");
        }

        public async Task<HttpResponseMessage> GetPostByUserId(string id)
        {
            return await _client.GetAsync($"api/Post?UserId={id}");
        }

        public async Task<HttpResponseMessage> GetPostByFriends(ProfileViewModel profile)
        {
            return await _client.GetAsync($"api/Post?friends={profile.Email}");
        }

        public async Task<HttpResponseMessage> UpdatePost(int Id, PostViewModel model)
        {
            return await _client.PutAsJsonAsync($"api/Post/{Id}", model);
        }

        public async Task<HttpResponseMessage> InsertPost(PostViewModel model)
        {
            return await _client.PostAsJsonAsync("api/Post", model);
        }

        public async Task<HttpResponseMessage> DeletePost(int id)
        {
            return await _client.DeleteAsync($"api/Post/{id}");
        }
        #endregion

        #region Comment
        public async Task<HttpResponseMessage> GetComment(int id)
        {
            return await _client.GetAsync($"api/Comment/{id}");
        }

        public async Task<HttpResponseMessage> UpdateComment(int Id, CommentViewModel model)
        {
            return await _client.PutAsJsonAsync($"api/Comment/{Id}", model);
        }

        public async Task<HttpResponseMessage> InsertComment(CommentViewModel model)
        {
            return await _client.PostAsJsonAsync("api/Comment", model);
        }

        public async Task<HttpResponseMessage> DeleteComment(int id)
        {
            return await _client.DeleteAsync($"api/Comment/{id}");
        }
        #endregion

        #region Gallery
        public async Task<HttpResponseMessage> GetGalleries(string UserId)
        {
            return await _client.GetAsync($"api/Gallery?UserId={UserId}");
        }

        public async Task<HttpResponseMessage> GetGallery(int id)
        {
            return await _client.GetAsync($"api/Gallery/{id}");
        }

        public async Task<HttpResponseMessage> UpdateGallery(int Id, GalleryViewModel model)
        {
            return await _client.PutAsJsonAsync($"api/Gallery/{Id}", model);
        }

        public async Task<HttpResponseMessage> InsertGallery(GalleryViewModel model)
        {
            return await _client.PostAsJsonAsync("api/Gallery", model);
        }

        public async Task<HttpResponseMessage> DeleteGallery(int id)
        {
            return await _client.DeleteAsync($"api/Gallery/{id}");
        }
        #endregion

        #region Image
        public async Task<HttpResponseMessage> GetImages(GalleryViewModel gallery)
        {
            return await _client.GetAsync($"api/Image?galleryId={gallery.Id}");
        }

        public async Task<HttpResponseMessage> GetImage(int id)
        {
            return await _client.GetAsync($"api/Image/{id}");
        }

        public async Task<HttpResponseMessage> UpdateImage(int Id, ImageViewModel model)
        {
            return await _client.PutAsJsonAsync($"api/Image/{Id}", model);
        }

        public async Task<HttpResponseMessage> InsertImage(ImageViewModel model)
        {
            return await _client.PostAsJsonAsync("api/Image", model);
        }

        public async Task<HttpResponseMessage> DeleteImage(int id)
        {
            return await _client.DeleteAsync($"api/Image/{id}");
        }
        #endregion

        #region Likes
        public async Task<HttpResponseMessage> GetLikeById(int id)
        {
            return await _client.GetAsync($"api/Likes/{id}");
        }

        public async Task<HttpResponseMessage> GetLikeByPostId(int PostId)
        {
            return await _client.GetAsync($"api/Likes?postIds=postids&postId={PostId}");
        }

        public async Task<HttpResponseMessage> InsertLike(LikeViewModel model)
        {
            return await _client.PostAsJsonAsync("api/Likes", model);
        }

        public async Task<HttpResponseMessage> DeleteLike(int id, string userId)
        {
            return await _client.DeleteAsync($"api/Likes/{id}?userId={userId}");
        }
        #endregion

        #region Friends
        public async Task<HttpResponseMessage> GetFriendsById(int id)
        {
            return await _client.GetAsync($"api/Friend/{id}");
        }

        public async Task<HttpResponseMessage> GetFriends(string id)
        {
            return await _client.GetAsync($"api/Friend?UserId={id}");
        }

        public async Task<HttpResponseMessage> InsertFriend(FriendViewModel model)
        {
            return await _client.PostAsJsonAsync("api/Friend", model);
        }

        public async Task<HttpResponseMessage> DeleteFriend(int id)
        {
            return await _client.DeleteAsync($"api/Friend/{id}");
        }
        #endregion
    }
}