using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Devpro.PictureOrganizer
{
  public abstract class PictureFolderTaskBase
  {
    protected static List<string> _extensionList = new List<string>() { ".jpg", ".jpeg", ".bmp", ".png" };

    public abstract void Execute(string directory, int? seconds);

    protected List<FileInfo> GetFileList(string directory, bool recursive)
    {
      var dirInfo = new DirectoryInfo(directory);
      if (!dirInfo.Exists) { return null; }
      return dirInfo
        .GetFiles("*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
        .Where(f => (f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                    && _extensionList.Contains(f.Extension.ToLower()))
        .ToList()
      ;
    }
  }
}
