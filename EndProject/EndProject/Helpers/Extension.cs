using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Helpers
{
   public static class Extension
        {
            public static bool IsImage(this IFormFile file)
            {
                return file.ContentType.Contains("image/");
            }
            public static bool IsMore4Mb(this IFormFile file)
            {
                return file.Length / 1024 > 4096;
            }
            public static async Task<string> SaveImageAsync(this IFormFile file, string path)
            {
                string filename = Guid.NewGuid().ToString() + file.FileName;
                string fullpath = Path.Combine(path, filename);
                using (FileStream fileStream = new FileStream(fullpath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return filename;
            }
        }
    public enum Roles
    {
        Admin,
        Member
    }
}

