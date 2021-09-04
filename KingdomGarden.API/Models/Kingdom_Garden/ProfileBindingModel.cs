using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KingdomGarden.API.Models.Kingdom_Garden
{
    public class ProfileBindingModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public IEnumerable<FriendBindingModel> Friends { get; set; }
    }

    public class PostBindingModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public int Likes { get; set; }
        public IEnumerable<CommentBindingModel> Comments { get; set; }
    }

    public class CommentBindingModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public int PostId { get; set; }
    }

    public class FriendBindingModel
    {
        public int Id { get; set; }
        public string UserId1 { get; set; }
        public string UserId2 { get; set; }
    }

    public class ImageBindingModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int GalleryId { get; set; }
    }

    public class LikeBindingModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
    }

    public class GalleryBindingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}