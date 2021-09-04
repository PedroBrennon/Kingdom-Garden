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
    public class CommentController : ApiController
    {
        private SqlConnection _connection;

        private void Connection()
        {
            string cons = ConfigurationManager.ConnectionStrings["kingdomgarden"].ToString();
            _connection = new SqlConnection(cons);
        }
        // GET: api/Comment/post
        public IEnumerable<CommentBindingModel> Get(PostBindingModel post)
        {
            Connection();
            List<CommentBindingModel> comments = new List<CommentBindingModel>();

            using (SqlCommand command = new SqlCommand("GetCommentsByPostId", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PostId", post.Id);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var comment = new CommentBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Text = Convert.ToString(reader["Text"]),
                            UserId = Convert.ToString(reader["UserId"]),
                            ImageUrl = Convert.ToString(reader["Url"]),
                            PostId = Convert.ToInt32(reader["PostId"])
                        };
                        comments.Add(comment);
                    }
                }
                _connection.Close();

                return comments;
            }
        }

        // GET: api/Comment/5
        public CommentBindingModel Get(int id)
        {
            Connection();
            CommentBindingModel comment = new CommentBindingModel();

            using (SqlCommand command = new SqlCommand("GetCommentsById", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        comment = new CommentBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Text = Convert.ToString(reader["Text"]),
                            UserId = Convert.ToString(reader["UserId"]),
                            ImageUrl = Convert.ToString(reader["Url"]),
                            PostId = Convert.ToInt32(reader["PostId"])
                        };
                    }
                }
                _connection.Close();

                return comment;
            }
        }

        // POST: api/Comment
        public void Post(CommentBindingModel comment)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("InsertComment", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Text", comment.Text ?? Convert.DBNull);
                command.Parameters.AddWithValue("@ImageUrl", comment.ImageUrl ?? Convert.DBNull);
                command.Parameters.AddWithValue("@UserId", comment.UserId);
                command.Parameters.AddWithValue("@PostId", comment.PostId);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // PUT: api/Comment/5
        public void Put(int id, CommentBindingModel comment)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("UpdateComment", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", comment.Id);
                command.Parameters.AddWithValue("@Text", comment.Text ?? Convert.DBNull);
                command.Parameters.AddWithValue("@ImageUrl", comment.ImageUrl ?? Convert.DBNull);
                command.Parameters.AddWithValue("@UserId", comment.UserId);
                command.Parameters.AddWithValue("@PostId", comment.PostId);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // DELETE: api/Comment/5
        public void Delete(int id)
        {
            Connection();
            using (SqlCommand command = new SqlCommand("DeleteComments", _connection))
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
