using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Devpro.PictureOrganizer
{
  public class MovePictureDateTask : PictureFolderTaskBase
  {
    public override void Execute(string directory, int? seconds)
    {
      MovePictureDate(directory, seconds.Value, true);
    }

    protected void MovePictureDate(string directory, int seconds, bool recursive)
    {
      var dirInfo = new DirectoryInfo(directory);
      foreach (var fileInfo in dirInfo.GetFiles("*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)) {
        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden
          || !_extensionList.Contains(fileInfo.Extension.ToLower())) {
          continue;
        }

        var pictFile = new PictureFile(fileInfo.FullName);
        if (pictFile.HasRecordingDate()) {
          pictFile.MoveShootingDate(seconds);
          pictFile.ApplyChanges();
          fileInfo.CreationTime = pictFile.ShootingDate;
        }
      }
    }
  }
}
