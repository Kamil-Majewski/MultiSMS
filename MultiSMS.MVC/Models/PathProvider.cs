using MultiSMS.BusinessLogic.Services.Interfaces;

namespace MultiSMS.MVC.Models
{
    public class PathProvider : IPathProvider
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PathProvider(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string WwwRootPath => _webHostEnvironment.WebRootPath;
    }
}
