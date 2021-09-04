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
    public class FriendController : ApiController
    {
        private SqlConnection _connection;

        private void Connection()
        {
            string cons = ConfigurationManager.ConnectionStrings["kingdomgarden"].ToString();
            _connection = new SqlConnection(cons);
        }

        // GET: api/Friend/5
        public FriendBindingModel Get(int id)
        {
            Connection();
            var friends = new FriendBindingModel();

            using (SqlCommand command = new SqlCommand("GetFriendsById", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        friends = new FriendBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UserId1 = Convert.ToString(reader["UserId1"]),
                            UserId2 = Convert.ToString(reader["UserId2"])
                        };
                    }
                }
                _connection.Close();
            }

            return friends;
        }

        // GET: api/Friend?UserId=string
        public IEnumerable<FriendBindingModel> Get(string UserId)
        {
            Connection();
            var friends = new List<FriendBindingModel>();

            using (SqlCommand command = new SqlCommand("GetFriendsByUserId", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId1", UserId);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var friend = new FriendBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UserId1 = Convert.ToString(reader["UserId1"]),
                            UserId2 = Convert.ToString(reader["UserId2"])
                        };
                        friends.Add(friend);
                    }
                }
                _connection.Close();
            }

            return friends;
        }

        // POST: api/Friend
        public void Post(FriendBindingModel friend)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("InsertFriend", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId1", friend.UserId1);
                command.Parameters.AddWithValue("@UserId2", friend.UserId2);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // PUT: api/Friend/5
        /*public void Put(int id, [FromBody]string value)
        {
        }*/

        // DELETE: api/Friend/5
        public void Delete(int id)
        {
            Connection();
            using (SqlCommand command = new SqlCommand("DeleteFriends", _connection))
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
