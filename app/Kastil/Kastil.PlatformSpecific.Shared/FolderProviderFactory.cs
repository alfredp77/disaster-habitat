using System;
using System.Collections.Generic;
using System.Text;
using Kastil.Core;

namespace Kastil.PlatformSpecific.Shared
{
    public static class FolderProviderFactory
    {
        public static FolderProvider Create()
        {
            return new FolderProvider
            {
                MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
        }
    }
}
