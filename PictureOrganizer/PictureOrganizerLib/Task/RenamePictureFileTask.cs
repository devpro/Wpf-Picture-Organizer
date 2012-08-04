using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Devpro.PictureOrganizer
{
  public class RenamePictureFileTask : PictureFolderTaskBase
  {
    public string Directory  { get; set; }
    public bool   Recursive  { get; set; }
    public string FileFormat { get; set; }
    public string DateFormat { get; set; }

    public RenamePictureFileTask()
    {
      Recursive  = true;
      DateFormat = "dd-HHmmss";
    }

    public override void Execute(string directory, int? seconds)
    {
      if (string.IsNullOrEmpty(Directory) || string.IsNullOrEmpty(FileFormat)) {
        throw new Exception("Directory or file format is undefined");
      }
      if (!FileFormat.Contains("{0}")) {
        throw new Exception("File format is invalid");
      }
      var files = GetFileList(Directory, Recursive);
      if (files == null) {
        throw new Exception("Directory does not exist or files cannot be retrieved");
      }

      var nbFiles = files.Count;
      foreach (var fileInfo in files) {
        var pictFile = new PictureFile(fileInfo.FullName);
        fileInfo.MoveTo(Path.Combine(
          fileInfo.DirectoryName,
          string.Format(FileFormat, fileInfo.Name, pictFile.ShootingDate.ToString(DateFormat))
        ));
      }
    }
  }
}
