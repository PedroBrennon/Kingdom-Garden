using KingdomGarden.API.Models.Kingdom_Garden;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KingdomGarden.API.Controllers.Kingdom__Garden
{
    public class PostController : ApiController
    {
        private SqlConnection _connection;

        private void Connection()
        {
            string cons = ConfigurationManager.ConnectionStrings["kingdomgarden"].ToString();
            _connection = new SqlConnection(cons);
        }

        // GET: api/Post/5
        public PostBindingModel Get(int id)
        {
            Connection();
            PostBindingModel post = new PostBindingModel();
            var likesController = new LikesController();
            var commentsController = new CommentController();

            using (SqlCommand command = new SqlCommand("GetPostById", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        post = new PostBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Text = Convert.ToString(reader["Text"]),
                            UserId = Convert.ToString(reader["UserId"]),
                            ImageUrl = Convert.ToString(reader["Url"]),
                            CreateDate = Convert.ToDateTime(reader["CreateDate"])
                        };
                    }
                }
                post.Likes = likesController.GetLikesPost(post.Id);
                post.Comments = commentsController.Get(post);
                _connection.Close();

                return post;
            }
        }

        // GET: api/Post?UserId=string
        public IEnumerable<PostBindingModel> Get(string UserId)
        {
            Connection();
            List<PostBindingModel> posts = new List<PostBindingModel>();
            var likesController = new LikesController();
            var commentsController = new CommentController();

            using (SqlCommand command = new SqlCommand("GetPostsByUserId", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var post = new PostBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Text = Convert.ToString(reader["Text"]),
                            UserId = Convert.ToString(reader["UserId"]),
                            ImageUrl = Convert.ToString(reader["Url"]),
                            CreateDate = Convert.ToDateTime(reader["CreateDate"])
                        };
                        post.Likes = likesController.GetLikesPost(post.Id);
                        post.Comments = commentsController.Get(post);
                        posts.Add(post);
                    }
                }
                _connection.Close();

                return posts;
            }
        }

        // GET: api/Post?friends=
        public IEnumerable<PostBindingModel> Get(ProfileBindingModel profile, string friends)
        {
            Connection();
            List<PostBindingModel> posts = new List<PostBindingModel>();
            var likesController = new LikesController();
            var commentsController = new CommentController();

            using (SqlCommand command = new SqlCommand("GetPostsByFriends", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", friends);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var post = new PostBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Text = Convert.ToString(reader["Text"]),
                            UserId = Convert.ToString(reader["UserId"]),
                            ImageUrl = Convert.ToString(reader["Url"]),
                            CreateDate = Convert.ToDateTime(reader["CreateDate"])
                        };
                        post.Likes = likesController.GetLikesPost(post.Id);
                        post.Comments = commentsController.Get(post);
                        posts.Add(post);
                    }
                }
                _connection.Close();

                return posts;
            }
        }

        // POST: api/Post
        public void Post(PostBindingModel post)
        {
            Connection();
            var now = DateTime.Now;

            using (SqlCommand command = new SqlCommand("InsertPost", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Text", post.Text);
                command.Parameters.AddWithValue("@ImageUrl", post.ImageUrl ?? Convert.DBNull);
                command.Parameters.AddWithValue("@UserId", post.UserId);
                command.Parameters.AddWithValue("@CreateDate", now);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // PUT: api/Put/5
        public void Put(int id, PostBindingModel post)
        {
            Connection();
            var now = DateTime.Now;

            using (SqlCommand command = new SqlCommand("UpdatePost", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Text", post.Text ?? Convert.DBNull);
                command.Parameters.AddWithValue("@ImageUrl", post.ImageUrl ?? Convert.DBNull);
                command.Parameters.AddWithValue("@UserId", post.UserId);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@CreateDate", now);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // DELETE: api/Post/5
        public void Delete(int id)
        {
            Connection();
            using (SqlCommand command = new SqlCommand("DeletePosts", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }
    }
}
