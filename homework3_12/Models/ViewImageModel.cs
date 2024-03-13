using homework3_12Data;

namespace homework3_12.Models
{
    public class ViewImageModel
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public bool IsLocked { get; set; }

        public Image Image { get; set; }
    }
}
