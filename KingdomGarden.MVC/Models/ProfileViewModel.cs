using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KingdomGarden.MVC.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Informe o Email")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "O campo Nome permite no máximo 50 caracteres!")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [DisplayName("Picture")]
        public string ImageUrl { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
    }

    public class PostViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        
        public string Text { get; set; }

        [DisplayName("Picture")]
        public string ImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public int Likes { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }
    }

    public class CommentViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Text { get; set; }

        [DisplayName("Picture")]
        public string ImageUrl { get; set; }

        [Required]
        public int PostId { get; set; }
    }

    public class FriendViewModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("My user")]
        public string UserId1 { get; set; }

        [Required]
        [DisplayName("Friend")]
        public string UserId2 { get; set; }
    }

    public class ImageViewModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Picture")]
        public string Url { get; set; }

        [Required]
        public int GalleryId { get; set; }
    }

    public class LikeViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int PostId { get; set; }
    }

    public class GalleryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}