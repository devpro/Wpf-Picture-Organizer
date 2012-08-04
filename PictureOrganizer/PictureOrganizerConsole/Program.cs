using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devpro.PictureOrganizer.Console
{
  using Console = System.Console;

  /// <summary>
  /// Picture organizer console.
  /// </summary>
  /// <example>Devpro.PictureOrganizerCons.exe "C:\\Users\\Bertrand\\Downloads\\tests\\Microsoft_headquarters_webanswers.jpg" property-overview</example>
  /// <example>Devpro.PictureOrganizerCons.exe "C:\\Users\\Bertrand\\Downloads\\tests" reset-date</example>
  /// <example>Devpro.PictureOrganizerCons.exe "C:\\Users\\Bertrand\\Downloads\\tests" move-date 60</example>
  /// <remarks>Devpro.PictureOrganizerCons.exe "C:\\Users\\Bertrand\\Downloads\\tests" rename "{date}_{file}"</remarks>
  class Program
  {
    static void Main(string[] args)
    {
      var nbArgs = args.Count();
      if (nbArgs < 1) {
        printUsage();
        return;
      }
      var filepath = args[0];
      PictureFolderTaskBase task;
      if (nbArgs > 1) {
        switch (args[1]) {
          case "move-date-test":
            MoveDate(filepath, 60); // tests
            break;
          case "rename":
            var fileFormat = args[2].Replace("{file}", "{0}").Replace("{date}", "{1}");
            task = new RenamePictureFileTask { Directory = filepath, FileFormat = fileFormat };
            task.Execute(null, null);
            break;
          case "reset-date":
            task = new ResetPictureFileDateTask();
            task.Execute(filepath, null);
            break;
          case "move-date":
            task = new MovePictureDateTask();
            var delay = nbArgs > 2 ? int.Parse(args[2]) : 60;
            task.Execute(filepath, delay);
            break;
          case "property-overview":
            DisplayOverview(filepath);
            break;
          default:
            printUsage();
            break;
        }
      }
    }

    private static void printUsage()
    {
      Console.WriteLine(string.Format(
        "Usage: {0} path/to/picture {1}",
        "Devpro.PictureOrganizerCons.exe",
        "property-overview|reset-date|move-date|rename"
      ));
    }

    private static void MoveDate(string filepath, int seconds)
    {
      var file = new PictureFile(filepath);

      Console.WriteLine(file.RecordingDate);
      Console.WriteLine(file.ShootingDate);

      file.MoveShootingDate(seconds);
      file.ApplyChanges();

      Console.WriteLine(file.RecordingDate);
      Console.WriteLine(file.ShootingDate);
    }

    private static void DisplayOverview(string filepath)
    {
      var propView = PictureFile.GetPropertyOverview(filepath);
      propView.ToList().ForEach(p => Console.WriteLine(p.Value));
    }
  }
}
