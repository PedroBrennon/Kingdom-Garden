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
    public class LikesController : ApiController
    {
        private SqlConnection _connection;

        private void Connection()
        {
            string cons = ConfigurationManager.ConnectionStrings["kingdomgarden"].ToString();
            _connection = new SqlConnection(cons);
        }

        // GET: api/Likes?postIds=string&postId=ID
        public IEnumerable<LikeBindingModel> Get(string postIds, int postId)
        {
            Connection();
            var likes = new List<LikeBindingModel>();

            using (SqlCommand command = new SqlCommand("GetLikesByPostId", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PostId", postId);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var like = new LikeBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UserId = Convert.ToString(reader["UserId"]),
                            PostId = Convert.ToInt32(reader["PostId"])
                        };
                        likes.Add(like);
                    }
                }
                _connection.Close();
            }
            return likes;
        }

        public LikeBindingModel Get(int id)
        {
            Connection();
            var like = new LikeBindingModel();

            using (SqlCommand command = new SqlCommand("GetLikesById", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        like = new LikeBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UserId = Convert.ToString(reader["UserId"]),
                            PostId = Convert.ToInt32(reader["PostId"])
                        };
                    }
                }
                _connection.Close();
            }
            return like;
        }
        
        public int GetLikesPost(int postId)
        {
            Connection();
            int likes = 0;

            using (SqlCommand command = new SqlCommand("GetLikesCountByPostId", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PostId", postId);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    likes = Convert.ToInt32(reader["Count"]);
                }
                _connection.Close();
            }

            return likes;
        }

        // POST: api/Likes
        public void Post(LikeBindingModel like)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("InsertLike", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", like.UserId);
                command.Parameters.AddWithValue("@PostId", like.PostId);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // PUT: api/Likes/5
        /*public void Put(int id, [FromBody]string value)
        {
        }*/

        // DELETE: api/Likes/5?userId=
        public void Delete(int id, string userId)
        {
            Connection();
            using (SqlCommand command = new SqlCommand("DeleteLikes", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PostId", id);
                command.Parameters.AddWithValue("@UserId", userId);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }
    }
}
