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
    public class ProfileController : ApiController
    {
        private SqlConnection _connection;

        private void Connection()
        {
            string cons = ConfigurationManager.ConnectionStrings["kingdomgarden"].ToString();
            _connection = new SqlConnection(cons);
        }

        // GET: api/Profile?count=0
        public int Get(int count)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("GetProfilesCount", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    count = Convert.ToInt32(reader["Count"]);
                }
                _connection.Close();
            }

            return count;
        }

        // GET: api/Profile?userId=userId
        public IEnumerable<ProfileBindingModel> Get(ProfileBindingModel profile, string userId)
        {
            Connection();
            var profiles = new List<ProfileBindingModel>();
            var friendController = new FriendController();

            using (SqlCommand command = new SqlCommand("GetAllProfiles", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userId", userId);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var p = new ProfileBindingModel
                        {
                            FirstName = Convert.ToString(reader["FirstName"]),
                            LastName = Convert.ToString(reader["LastName"]),
                            Email = Convert.ToString(reader["Email"]),
                            ImageUrl = Convert.ToString(reader["Url"]),
                            BirthDate = Convert.ToDateTime(reader["BirthDate"])
                        };
                        p.Friends = friendController.Get(p.Email);
                        profiles.Add(p);
                    }
                }
                _connection.Close();

                return profiles;
            }
        }

        // GET: api/Profile/string
        public ProfileBindingModel Get(string id)
        {
            Connection();
            ProfileBindingModel profile = new ProfileBindingModel();
            var friendController = new FriendController();

            using (SqlCommand command = new SqlCommand("GetProfileById", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if(reader.HasRows)
                    {
                        profile = new ProfileBindingModel
                        {
                            FirstName = Convert.ToString(reader["FirstName"]),
                            LastName = Convert.ToString(reader["LastName"]),
                            Email = Convert.ToString(reader["Email"]),
                            ImageUrl = Convert.ToString(reader["Url"]),
                            BirthDate = Convert.ToDateTime(reader["BirthDate"])
                        };
                        profile.Friends = friendController.Get(id);
                    }
                }
                _connection.Close();

                return profile;
            }
        }

        // POST: api/Profile
        public void Post(ProfileBindingModel profile)
        {
            Connection();

            if (profile.BirthDate.Equals(new DateTime(0001, 01, 01))) { profile.BirthDate = DateTime.Now; }

            using (SqlCommand command = new SqlCommand("InsertProfile", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ImageUrl", profile.ImageUrl ?? Convert.DBNull);
                command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                command.Parameters.AddWithValue("@LastName", profile.LastName ?? Convert.DBNull);
                command.Parameters.AddWithValue("@UserId", profile.Email);
                command.Parameters.AddWithValue("@BirthDate", profile.BirthDate);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // PUT: api/Profile/5
        public void Put(string id, ProfileBindingModel profile)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("UpdateProfile", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ImageUrl", profile.ImageUrl ?? Convert.DBNull);
                command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                command.Parameters.AddWithValue("@LastName", profile.LastName ?? Convert.DBNull);
                command.Parameters.AddWithValue("@UserId", id);
                command.Parameters.AddWithValue("@BirthDate", profile.BirthDate);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // DELETE: api/Profile/5
        public void Delete(string id)
        {
            Connection();
            using (SqlCommand command = new SqlCommand("DeleteProfiles", _connection))
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
