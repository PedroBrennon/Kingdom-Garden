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
    public class ImageController : ApiController
    {
        private SqlConnection _connection;

        private void Connection()
        {
            string cons = ConfigurationManager.ConnectionStrings["kingdomgarden"].ToString();
            _connection = new SqlConnection(cons);
        }

        // GET: api/Image?galleryId=
        public IEnumerable<ImageBindingModel> Get(GalleryBindingModel gallery, int galleryId)
        {
            Connection();
            List<ImageBindingModel> images = new List<ImageBindingModel>();

            using (SqlCommand command = new SqlCommand("GetImagesByGalleryId", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", galleryId);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var image = new ImageBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Url = Convert.ToString(reader["Url"]),
                            GalleryId = Convert.ToInt32(reader["GalleryId"])
                        };
                        images.Add(image);
                    }
                }
                _connection.Close();

                return images;
            }
        }

        // GET: api/Image/5
        public ImageBindingModel Get(int id)
        {
            Connection();
            ImageBindingModel image = new ImageBindingModel();

            using (SqlCommand command = new SqlCommand("GetImagesById", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                _connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        image = new ImageBindingModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Url = Convert.ToString(reader["Url"]),
                            GalleryId = Convert.ToInt32(reader["GalleryId"])
                        };
                    }
                }
                _connection.Close();

                return image;
            }
        }

        // POST: api/Image
        public void Post(ImageBindingModel image)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("InsertImage", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Url", image.Url);
                command.Parameters.AddWithValue("@GalleryId", image.GalleryId);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // PUT: api/Image/5
        public void Put(int id, ImageBindingModel image)
        {
            Connection();

            using (SqlCommand command = new SqlCommand("UpdateImage", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", image.Id);
                command.Parameters.AddWithValue("@Url", image.Url);
                command.Parameters.AddWithValue("@GalleryId", image.GalleryId);

                _connection.Open();
                int execute = command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        // DELETE: api/Image/5
        public void Delete(int id)
        {
            Connection();
            using (SqlCommand command = new SqlCommand("DeleteImages", _connection))
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
