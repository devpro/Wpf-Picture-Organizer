using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Devpro.PictureOrganizer
{
  public class PictureFile
  {
    private enum ImagePropNames
    {
      SHOT_DATE = 0
    }

    private Dictionary<ImagePropNames, int[]> IMAGE_PROPERTIES = new Dictionary<ImagePropNames, int[]>();

    private ASCIIEncoding _encoding = new ASCIIEncoding();

    /// <summary>
    /// Date du cliché.
    /// </summary>
    public DateTime ShotDate { get; private set; }

    /// <summary>
    /// Chemin du fichier.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// Private constructor.
    /// </summary>
    private PictureFile()
    {
      IMAGE_PROPERTIES[ImagePropNames.SHOT_DATE] = new int[2] { 2, 36867 };
    }

    /// <summary>
    /// Public constructor.
    /// </summary>
    /// <param name="filePath">picture file path</param>
    public PictureFile(string filePath)
      : this()
    {
      FilePath = filePath;

      LoadFile();
    }

    private void LoadFile()
    {
      // found here: http://www.csharpfr.com/forum/sujet-RECUPERATION-RESUME-FICHIER-IMAGE_216530.aspx

      var img = Image.FromFile(FilePath);

      // la date peut être située à n'importe quelle position
      foreach (PropertyItem propItem in img.PropertyItems) {
        if (propItem.Type == IMAGE_PROPERTIES[ImagePropNames.SHOT_DATE][0]
         && propItem.Id == IMAGE_PROPERTIES[ImagePropNames.SHOT_DATE][1]) {
          // la date du cliché (id 36867) est au format ASCII (type 2)
          //ShotDate = DateTime.Parse(_encoding.GetString(propItem.Value, 0, propItem.Len - 1), ); // "2007:09:05 11:36:45"	string
          ShotDate = DateTime.ParseExact(_encoding.GetString(propItem.Value, 0, propItem.Len - 1),
                                         "yyyy:MM:dd HH:mm:ss",
                                         CultureInfo.CurrentCulture);
        }
      }

      img.Dispose();
    }

    /// <summary>
    /// Apply changes.
    /// </summary>
    public void ApplyChanges()
    {
      // found here: http://stackoverflow.com/questions/336387/image-save-throws-a-gdi-exception-because-the-memory-stream-is-closed

      var data = File.ReadAllBytes(FilePath);
      var originalBinaryDataStream = new MemoryStream(data);

      Bitmap image = new Bitmap(originalBinaryDataStream);

      foreach (PropertyItem propItem in image.PropertyItems) {
        if (propItem.Type == IMAGE_PROPERTIES[ImagePropNames.SHOT_DATE][0]
           && propItem.Id == IMAGE_PROPERTIES[ImagePropNames.SHOT_DATE][1]) {
          propItem.Value = _encoding.GetBytes(ShotDate.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture));
          image.SetPropertyItem(propItem);
        }
      }

      image.Save(FilePath);

      image.Dispose();
    }

    /// <summary>
    /// Move picture shot date.
    /// </summary>
    /// <param name="deltaTime">delta time in seconds</param>
    internal void MoveShotDate(int deltaTime)
    {
      ShotDate = ShotDate.AddSeconds(deltaTime);
    }
  }
}
