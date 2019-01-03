using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EStalls.Services.Items
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHostingEnvironment _environment;


        public ItemService(
            IHostingEnvironment environment,
            ApplicationDbContext context,
            SignInManager<AppUser> signInManager)
        {
            _environment = environment;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task SaveItemAsync(InputItemModel inputItem)
        {
            var registrationTime = DateTime.Now;

            var claims = this._signInManager.Context.User;
            var user = await this._signInManager.UserManager.GetUserAsync(claims);
            var uid = user.Id;

            var previewFileNames = new List<string>();
            foreach (var previewFile in inputItem.PreviewFiles)
            {
                var safeName = WebUtility.HtmlEncode(Path.GetFileName(previewFile.FileName));

                previewFileNames.Add(safeName);
            }

            var dlFileNames = new List<string>();
            foreach (var dlFile in inputItem.DlFiles)
            {
                var safeName = WebUtility.HtmlEncode(Path.GetFileName(dlFile.FileName));

                dlFileNames.Add(safeName);
            }

            var safeThumbFileName = WebUtility.HtmlEncode(Path.GetFileName(inputItem.ThumbnailFile.FileName));

            var item = await _context.Item
                .AddAsync(new Item()
                {
                    Id = Guid.NewGuid(),
                    Uid = uid,
                    Title = inputItem.Title,
                    Description = inputItem.Description,
                    Price = inputItem.Price,
                    PreviewFileNames = string.Join(",", previewFileNames),
                    ThumbnailFileName = safeThumbFileName,
                    RegistrationTime = registrationTime,
                    UpdateTime = registrationTime
                });

            var itemDlInfo = await _context.ItemDlInfo
                .AddAsync(new ItemDlInfo()
                {
                    Id = Guid.NewGuid(),
                    ItemId = item.Entity.Id,
                    Version = inputItem.Version,
                    DlFileNames = string.Join(",", dlFileNames),
                    RegistrationTime = item.Entity.RegistrationTime,
                    UpdateTime = item.Entity.UpdateTime
                });

            await _context.SaveChangesAsync();


            // TODO: ファイルの保存をUtility化
            var dirPath = Path.Combine(new []
            {
                Constants.DirNames.ItemPreviewFiles,
                item.Entity.Id.ToString()
            });

            for (var i = 0; i < inputItem.PreviewFiles.Count; i++)
            {
                var file = inputItem.PreviewFiles[i];
                var fileName = previewFileNames[i];

                var filePath = await this.SaveFileAsync(file, dirPath, fileName);
            }


            var thumbDirPath = Path.Combine(new[]
            {
                dirPath,
                Constants.DirNames.ItemThumbnailFile
            });
            var thumbFilePath = await this.SaveFileAsync(inputItem.ThumbnailFile, thumbDirPath, safeThumbFileName);


            var dlDirPath = Path.Combine(new []
            {
                Constants.DirNames.ItemDlFiles,
                itemDlInfo.Entity.Id.ToString()
            });

            for (var i = 0; i < inputItem.DlFiles.Count; i++)
            {
                var file = inputItem.DlFiles[i];
                var fileName = dlFileNames[i];

                var filePath = await this.SaveFileAsync(file, dlDirPath, fileName);
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file, string dirPath, string fileName)
        {
            dirPath = Path.Combine(this._environment.WebRootPath, dirPath);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var filePath = Path.Combine(dirPath, fileName);

            if (file.Length > 0)
            {
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

                    return null;
                }
            }

            return filePath;
        }
    }
}
