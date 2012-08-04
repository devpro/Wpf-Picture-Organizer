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
  public class ResetPictureFileDateTask : PictureFolderTaskBase
  {
    public override void Execute(string directory, int? seconds)
    {
      var dirInfo = new DirectoryInfo(directory);
      foreach (var fileInfo in dirInfo.GetFiles("*.*", SearchOption.AllDirectories)) {
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
