using Microsoft.AspNetCore.Http;

namespace MiniProject.Models
{
    public class UploadedFile
    {
        private IFormFile _file;

        public UploadedFile(IFormFile file)
        {
            _file = file;
        }

        public string Save()
        {
            var fileName = System.IO.Path.GetFileName(_file.FileName);

            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            using (var localFile = System.IO.File.OpenWrite(fileName))
            {
                using (var savedFile = _file.OpenReadStream())
                {
                    savedFile.CopyTo(localFile);
                }
            }

            return fileName;
        }

        
    }
}
