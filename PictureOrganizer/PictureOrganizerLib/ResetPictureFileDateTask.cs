using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Devpro.PictureOrganizer
{
  /// <summary>
  /// Reset picture file date task.
  /// </summary>
  public class ResetPictureFileDateTask
  {
    private static List<string> _extensionList = new List<string>() { ".jpg", ".jpeg", ".bmp", ".png" };

    public void Execute(string directory)
    {
      var dirInfo = new DirectoryInfo(directory);
      foreach (var fileInfo in dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly)) {
        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden
          || !_extensionList.Contains(fileInfo.Extension.ToLower())) {
          continue;
        }

        var pictFile = new PictureFile(fileInfo.FullName);
        if (pictFile.HasRecordingDate())
        {
          fileInfo.CreationTime = pictFile.HasShootingDate() ? pictFile.ShootingDate : pictFile.RecordingDate;
        }
      }
    }
  }
}
