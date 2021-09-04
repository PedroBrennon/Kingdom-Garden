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
    public class GalleryController : ApiController
    {
        private SqlConnection _connection;

        private void Connection()
        {
            string cons = ConfigurationManager.ConnectionStrings["kingdomgarden"].ToString();
            _connection = new SqlConnection(cons);
        }

        // GET: api/Gallery
        public IEnumerable<GalleryBindingModel> Get(string UserId)
        {
            Connection();
            List<GalleryBindingModel> galleries = new List<GalleryBindingModel>();

            using (SqlCommand command = new SqlCommand("GetGalleryByUserId", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var gallery = new GalleryBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = Convert.ToString(reader["Name"]),
                            UserId = Convert.ToString(reader["UserId"])
                        };
                        galleries.Add(gallery);
                    }
                }
                _connection.Close();

                return galleries;
            }
        }

        // GET: api/Gallery/5
        public GalleryBindingModel Get(int id)
        {
            Connection();
            GalleryBindingModel gallery = new GalleryBindingModel();

            using (SqlCommand command = new SqlCommand("GetGalleryById", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        gallery = new GalleryBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = Convert.ToString(reader["Name"]),
                            UserId = Convert.ToString(reader["UserId"])
                        };
                    }
                }
                _connection.Close();

                return gallery;
            }

        }

        // POST: api/Gallery
        public void Post(GalleryBindingModel gallery)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("InsertGallery", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Name", gallery.Name);
                command.Parameters.AddWithValue("@UserId", gallery.UserId);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // PUT: api/Gallery/5
        public void Put(int id, GalleryBindingModel gallery)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("UpdateGallery", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", gallery.Name);
                command.Parameters.AddWithValue("@UserId", gallery.UserId);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // DELETE: api/Gallery/5
        public void Delete(int id)
        {
            Connection();
            using (SqlCommand command = new SqlCommand("DeleteGallery", _connection))
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
