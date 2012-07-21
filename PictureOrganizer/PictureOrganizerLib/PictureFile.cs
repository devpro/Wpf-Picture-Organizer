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
  /// <summary>
  /// Picture file class.
  /// </summary>
  public class PictureFile
  {
    // article: http://www.csharpfr.com/forum/sujet-RECUPERATION-RESUME-FICHIER-IMAGE_216530.aspx
    // article: http://stackoverflow.com/questions/336387/image-save-throws-a-gdi-exception-because-the-memory-stream-is-closed

    private enum ImagePropertyType
    {
      STRING = 2 // array of Byte objects encoded as ASCII
    }

    private enum ImagePropertyKey
    {
      RECORDING = 306,
      SHOOTING1 = 36867,
      SHOOTING2 = 36868
      // 37521
      // 37522
    }

    private static ASCIIEncoding _encoding = new ASCIIEncoding();

    private enum ImagePropNames
    {
      SHOT_DATE = 0
    }
    private Dictionary<ImagePropNames, int[]> IMAGE_PROPERTIES = new Dictionary<ImagePropNames, int[]>();

    /// <summary>
    /// Recording date (prise de vue).
    /// </summary>
    public DateTime RecordingDate { get; private set; }

    /// <summary>
    /// Shooting date (date cliché).
    /// </summary>
    public DateTime ShootingDate { get; private set; }

    /// <summary>
    /// File path.
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

    /// <summary>
    /// Apply changes.
    /// </summary>
    public void ApplyChanges()
    {
      var data                     = File.ReadAllBytes(FilePath);
      var originalBinaryDataStream = new MemoryStream(data);
      var image                    = new Bitmap(originalBinaryDataStream);

      try {
        foreach (var propItem in image.PropertyItems) { // PropertyItem
          if (HasRecordingDate()) {
            var recording   = image.GetPropertyItem((int)ImagePropertyKey.RECORDING);
            recording.Value = _encoding.GetBytes(RecordingDate.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture));
            image.SetPropertyItem(recording);
            var shooting1   = image.GetPropertyItem((int)ImagePropertyKey.SHOOTING1);
            shooting1.Value = _encoding.GetBytes(ShootingDate.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture));
            image.SetPropertyItem(shooting1);
            var shooting2   = image.GetPropertyItem((int)ImagePropertyKey.SHOOTING2);
            shooting2.Value = _encoding.GetBytes(ShootingDate.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture));
            image.SetPropertyItem(shooting2);
          }
        }

        image.Save(FilePath);
      } finally {
        image.Dispose();
        originalBinaryDataStream.Dispose();
      }
    }

    /// <summary>
    /// Move picture shot date.
    /// </summary>
    /// <param name="deltaTime">delta time in seconds</param>
    public void MoveShootingDate(int deltaTime)
    {
      if (HasRecordingDate()) {
        ShootingDate = ShootingDate.AddSeconds(deltaTime);
      }
    }

    public bool HasRecordingDate()
    {
      return RecordingDate != default(DateTime);
    }

    public bool HasShootingDate()
    {
      return ShootingDate != default(DateTime);
    }

    /// <summary>
    /// Read properties.
    /// </summary>
    /// <param name="filepath">File path</param>
    /// <returns>Dictionary</returns>
    public static Dictionary<int, string> GetPropertyOverview(string filepath)
    {
      var img = Image.FromFile(filepath);
      try {
        var properties = img.PropertyItems
          .ToDictionary(p => p.Id, p => string.Format("{0} {1} {2}", p.Id, p.Type, p.Len))
        ;
        return properties;
      } finally {
        img.Dispose();
      }
    }

    /// <summary>
    /// Load file and get properties.
    /// </summary>
    private void LoadFile()
    {
      var properties = ReadProperties();

      var recording = properties.SingleOrDefault(p => p.Key == (int)ImagePropertyKey.RECORDING);
      if (recording.Value != null) {
        RecordingDate = DateTime.ParseExact(recording.Value, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture);
      }

      var shooting = properties.SingleOrDefault(p => p.Key == (int)ImagePropertyKey.SHOOTING1);
      if (shooting.Value != null) {
        ShootingDate = DateTime.ParseExact(shooting.Value, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture);
      } else if (HasRecordingDate()) {
        ShootingDate = DateTime.Parse(RecordingDate.ToString());
      }
    }

    /// <summary>
    /// Read properties.
    /// </summary>
    /// <returns>Dictionary</returns>
    private Dictionary<int, string> ReadProperties()
    {
      var img = Image.FromFile(FilePath); // throws FileNotFoundException
      try {
        var properties = img.PropertyItems
          .Where(p => p.Type == (int)ImagePropertyType.STRING)
          .ToDictionary(p => p.Id, p => _encoding.GetString(p.Value, 0, p.Len - 1))
        ;
        return properties;
      } finally {
        img.Dispose();
      }
    }
  }
}
