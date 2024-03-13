using homework3_12.Models;
using homework3_12Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Text.Json;


namespace homework3_12.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=ImageWebsite;Integrated Security=True;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upload(IFormFile image, string password)
        {
            ImageRepository repo = new ImageRepository(_connectionString);

            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
            image.CopyTo(fs);

            string name = fileName;

            int id = repo.AddImage(name, password);
            UploadViewModel vm = new UploadViewModel();
            vm.Password = password;
            vm.Url = $"http://localhost:5112/home/viewimage?id={id}";
            return View(vm);
        }

        [HttpPost]
        public IActionResult ViewImage(int id, string password) 
        {
            ImageRepository repo = new ImageRepository(_connectionString);
            List<Image> images = repo.GetImages();

            bool AreAny = images.Any((i) => i.Id == id && i.Password == password);
           
                if (!AreAny)
                {
                TempData["incorrect"] = "incorrect password";
                    return Redirect($"/home/viewimage?id={id}");
                }
                Image i = repo.GetImage(id);
            repo.UpdateImage(id);
                ViewImageModel vm = new()
                {
                    Image = i,
                    Id = id,
                    IsLocked = false,
                };
            List<int> sessionIds = GetIdsFromSession();
            sessionIds.Add(id);


            return View(vm);
        }

       
        public IActionResult ViewImage(int id)
        {
           

            ViewImageModel vm = new()
            {
                Id = id,            

            };
            List<int> sessionIds = GetIdsFromSession();
            bool isAny = sessionIds.Any((s) => s == id);
            if (isAny)
            {
                vm.IsLocked = false;
            }
            else
            {
                vm.IsLocked = true;
            }

            if (TempData["incorrect"] != null)
            {
                vm.Message = (string)TempData["incorrect"];
            }
            return View(vm);
        }
        public List<int> GetIdsFromSession()
        {
            List<int> sessionIds = HttpContext.Session.Get<List<int>>("ids");
            if (sessionIds == null)
            {
                return new List<int>();
            }
            return sessionIds;
        }

    }

}
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);

        return value == null ? default(T) :
            JsonSerializer.Deserialize<T>(value);
    }
}