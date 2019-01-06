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
        public static bool CopyFile(
            string sourceDirPath, string sourceFileName, 
            string destDirPath, string destFileName)
        {
            if (string.IsNullOrWhiteSpace(sourceDirPath) ||
                string.IsNullOrWhiteSpace(sourceFileName) ||
                string.IsNullOrWhiteSpace(destDirPath) ||
                string.IsNullOrWhiteSpace(destFileName))
            {
                return false;
            }

            var sourceFilePath = Path.Combine(sourceDirPath, sourceFileName);

            if (!File.Exists(sourceFilePath))
            {
                return false;
            }

            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }

            var destFilePath = Path.Combine(destDirPath, destFileName);

            try
            {
                File.Copy(sourceFilePath, destFilePath, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }


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
            if (files == null)
            {
                return false;
            }

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


        public static void DeleteFile(string dirPath, string fileName)
        {
            var filePath = Path.Combine(dirPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }


        public static string GetHtmlEncodedFileName(IFormFile file)
        {
            return WebUtility.HtmlEncode(Path.GetFileName(file?.FileName)) ?? "";
        }


        public static string[] GetHtmlEncodedFileNames(IFormFile[] files)
        {
            return files?.Select(FileUtil.GetHtmlEncodedFileName)
                       .ToArray() ?? new string[] { };
        }
    }
}
