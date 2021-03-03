using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Commands
{
    public class CommandHelper
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public CommandHelper(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void UploadImage()
        {
            var files = "";
            byte[] b = File.ReadAllBytes(files);

            var contentRootPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Resource");

            DirectoryInfo contentDirectoryInfo;
            try
            {
                contentDirectoryInfo = new DirectoryInfo(contentRootPath);
            }
            catch (DirectoryNotFoundException)
            {
                // Here you should handle "Resources" directory not found exception.
                throw;
            }
        }

    }
}
