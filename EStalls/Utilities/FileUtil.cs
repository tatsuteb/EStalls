using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EStalls.Utilities
{
    public static class FileUtil
    {
        public static async Task<bool> SaveFileAsync(IFormFile file, string dirPath, string fileName = "")
        {
            if (fileName == "")
            {
                fileName = FileUtil.GetHtmlEncodedFileName(file);
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var filePath = Path.Combine(dirPath, fileName);

            if (file.Length <= 0) return false;

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);

                return false;
            }

            return true;
        }


        public static async Task<bool> SaveFilesAsync(IFormFile[] files, string dirPath, string[] fileNames = null)
        {
            if (fileNames == null ||
                fileNames.Length != files.Length)
            {
                fileNames = FileUtil.GetHtmlEncodedFileNames(files);
            }

            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var fileName = fileNames[i];

                if (!await FileUtil.SaveFileAsync(file, dirPath, fileName))
                {
                    return false;
                }
            }

            return true;
        }


        public static string GetHtmlEncodedFileName(IFormFile file)
        {
            return WebUtility.HtmlEncode(Path.GetFileName(file.FileName));
        }


        public static string[] GetHtmlEncodedFileNames(IFormFile[] files)
        {
            return files.Select(FileUtil.GetHtmlEncodedFileName)
                .ToArray();
        }
    }
}
