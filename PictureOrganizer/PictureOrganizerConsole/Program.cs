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
  /// <example>Devpro.PictureOrganizerCons.exe "C:\\Users\\Bertrand\\Downloads\\Microsoft_headquarters_webanswers.jpg" property-overview</example>
  /// <example>Devpro.PictureOrganizerCons.exe "C:\\Users\\Bertrand\\Downloads\\tests" reset-date</example>
  class Program
  {
    static void Main(string[] args)
    {
      var nbArgs   = args.Count();
      var filepath = string.Empty;
      if (nbArgs > 0) {
        filepath = args[0];
      }
      //filepath = "C:\\Users\\Bertrand\\Downloads\\Microsoft_headquarters_webanswers.jpg"

      if (nbArgs > 1) {
        switch (args[1]) {
          case "property-overview":
            DisplayOverview(filepath);
            break;
          case "reset-date":
            var task = new ResetPictureFileDateTask();
            task.Execute(filepath);
            break;
          default:
            MoveDate(filepath, 60); // tests
            break;
        }
      }
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
