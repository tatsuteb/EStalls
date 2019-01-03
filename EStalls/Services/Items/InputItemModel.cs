using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EStalls.Services.Items
{
    public class InputItemModel
    {
        #region 基本情報

        public string Title;
        public string Description;
        public int Price;
        public List<IFormFile> PreviewFiles;
        public IFormFile ThumbnailFile;

        #endregion

        #region ダウンロードデータ情報

        public string Version;
        public List<IFormFile> DlFiles;

        #endregion
    }
}
