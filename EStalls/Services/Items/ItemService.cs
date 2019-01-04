using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Models;
using EStalls.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EStalls.Services.Items
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<ItemService> _logger;

        public ItemService(
            IHostingEnvironment environment,
            ApplicationDbContext context,
            SignInManager<AppUser> signInManager,
            ILogger<ItemService> logger)
        {
            _environment = environment;
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task SaveItemAsync(InputItemModel inputItem)
        {
            var previewFileNames = FileUtil.GetHtmlEncodedFileNames(inputItem.PreviewFiles.ToArray());
            var dlFileNames = FileUtil.GetHtmlEncodedFileNames(inputItem.DlFiles.ToArray());
            var thumbFileName = FileUtil.GetHtmlEncodedFileName(inputItem.ThumbnailFile);

            var itemId = Guid.NewGuid();
            var itemDlInfoId = Guid.NewGuid();


            #region DBへ作品情報を保存

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var registrationTime = DateTime.Now;

                    var claims = this._signInManager.Context.User;
                    var user = await this._signInManager.UserManager.GetUserAsync(claims);
                    var uid = user.Id;

                    await _context.Item
                        .AddAsync(new Item()
                        {
                            Id = itemId,
                            Uid = uid,
                            Title = inputItem.Title,
                            Description = inputItem.Description,
                            Price = inputItem.Price,
                            PreviewFileNames = string.Join(",", previewFileNames),
                            ThumbnailFileName = thumbFileName,
                            RegistrationTime = registrationTime,
                            UpdateTime = registrationTime
                        });

                    await _context.ItemDlInfo
                        .AddAsync(new ItemDlInfo()
                        {
                            Id = itemDlInfoId,
                            ItemId = itemId,
                            Version = inputItem.Version,
                            DlFileNames = string.Join(",", dlFileNames),
                            RegistrationTime = registrationTime,
                            UpdateTime = registrationTime
                        });

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e.Message);

                    return;
                }
            }

            #endregion


            #region ファイルのアップロード

            try
            {
                // プレビューファイル
                var dirPath = Path.Combine(new[]
                {
                    this._environment.WebRootPath,
                    Constants.DirNames.ItemPreviewFiles,
                    itemId.ToString()
                });
                await FileUtil.SaveFilesAsync(inputItem.PreviewFiles.ToArray(), dirPath, previewFileNames);

                // サムネイルファイル
                var thumbDirPath = Path.Combine(new[]
                {
                    dirPath,
                    Constants.DirNames.ItemThumbnailFile
                });
                await FileUtil.SaveFileAsync(inputItem.ThumbnailFile, thumbDirPath, thumbFileName);

                // ダウンロード用ファイル
                var dlDirPath = Path.Combine(new[]
                {
                    this._environment.WebRootPath,
                    Constants.DirNames.ItemDlFiles,
                    itemDlInfoId.ToString()
                });
                await FileUtil.SaveFilesAsync(inputItem.DlFiles.ToArray(), dlDirPath, dlFileNames);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);

                return;
            }


            #endregion
        }

        
    }
}
