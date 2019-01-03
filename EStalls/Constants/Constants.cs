using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Constants
{
    public static class RoleTypes
    {
        public static string Seller => "Seller";
    }

    public static class PolicyTypes
    {
        public static string RequireSellerRole => "RequireSellerRole";
    }

    public static class DirNames
    {
        public static string ItemPreviewFiles => "items";
        public static string ItemThumbnailFile => "thumbnail";
        public static string ItemDlFiles => "files";
    }
}
