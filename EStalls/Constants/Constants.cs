﻿using System;
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
        public static string UserFiles => "users";

        public static string ItemPreviewFiles => "items";
        public static string ItemThumbnailFile => "thumbnail";
        public static string ItemDlFiles => "files";
    }

    public static class FileNames
    {
        public static string ProfileFileName => "profile";
    }

    public static class SessionKeys
    {
        public static string CartId => "_CartId";
        public static string CcToken => "_CcToken";
    }

    public static class StringSeparator
    {
        public static string DlFileNames => ",";
    }
}
