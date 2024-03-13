using System.Data.SqlClient;

namespace homework3_12Data
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int? Views { get; set; }
    }
    public class ImageRepository
    {

        private readonly string _connectionString;
        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;    
        }

        public int AddImage(string name, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"INSERT INTO Image(Name, Password, Views)
                                    VALUES(@name, @password, @views)SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@views", 0);
            conn.Open();
            return(int)(decimal)cmd.ExecuteScalar();
        }

        public Image GetImage(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"SELECT * FROM Image
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if(!reader.Read())
            {
                return null;
            }

            Image i = new Image()
            {
                Id = id,
                Name = (string)reader["Name"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"],
            };
            return i;
        }

        public void UpdateImage(int id)
        {

            int views = GetViews(id);
            views++;
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"UPDATE Image
                                    SET Views = @views
                                    WHERE Id = @id";
            cmd.Parameters.AddWithValue("@views", views);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();

        }

        public int GetViews(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"SELECT Views FROM Image 
                               WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public List<Image> GetImages()
        {
            using var conn = new SqlConnection(_connectionString);
                using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"SELECT * FROM Image";
            List<Image> images = new List<Image>();
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                images.Add(new()
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Password = (string)reader["Password"],
                    Views = (int)reader["Views"],
                });

            }
            return images;
        }
    }
}