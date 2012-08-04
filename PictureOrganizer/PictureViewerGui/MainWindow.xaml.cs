using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Devpro.PictureOrganizer;
using TextBox = System.Windows.Controls.TextBox;
using System.IO;
using ImageList = System.Windows.Forms.ImageList;
using System.ComponentModel;

namespace PictureViewerGui
{
  public class PictureData : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public string Filename { get; private set; }

    public string[] PathList { get; set; }

    private int _currentIndex;
    public int CurrentIndex
    {
      get { return _currentIndex; }
      set {
        _currentIndex = value;
        if (PathList != null) {
          Filename = PathList[_currentIndex];
        } else {
          Filename = null;
        }
        OnPropertyChanged("CurrentIndex");
      }
    }

    public void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged == null) { return; }
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private PictureData _pictureDataLeft;
    private PictureData _pictureDataRight;

    protected ImageList imgListLeft; // test

    public MainWindow()
    {
      InitializeComponent();

      //MessageBox.Show(imgLeft.Source.GetType().FullName);

      _pictureDataLeft = new PictureData();
      _pictureDataLeft.PropertyChanged += new PropertyChangedEventHandler(OnLeftPictureDataPropertyChanged);

      _pictureDataRight = new PictureData();
      _pictureDataRight.PropertyChanged += new PropertyChangedEventHandler(OnRightPictureDataPropertyChanged);
    }

    private void UpdatePictureInfo(Image img, TextBlock textBlock, string filename)
    {
      // SourceUpdated event on Image does not work...

      if (!string.IsNullOrEmpty(filename)) {
        img.Source          = new BitmapImage(new Uri(filename));
        textBlock.Text      = GetPictureDetail(filename);
      } else {
        img.Source          = null;
        textBlock.Text      = string.Empty;
      }
    }

    private string GetPictureDetail(string filepath)
    {
      var picFile = new PictureFile(filepath);
      return string.Format("Date : {0}", picFile.ShootingDate);
    }

    private string BrowseFolder(string selectedPath)
    {
      var dlg = new System.Windows.Forms.FolderBrowserDialog();
      if (!string.IsNullOrEmpty(selectedPath)) {
        dlg.SelectedPath = selectedPath;
      }
      var res = dlg.ShowDialog();
      if (res == System.Windows.Forms.DialogResult.OK) {
        return dlg.SelectedPath;
      }
      return null;
    }

    private void UpdateImage(TextBox textBox, TextBlock textBlock, Image image, PictureData pictureData)
    {
      var filepath = textBox.Text;
      FileInfo[] files = null;
      if (!string.IsNullOrEmpty(filepath)) {
        var dirInfo = new DirectoryInfo(filepath);
        if (dirInfo.Exists) {
          files = dirInfo.GetFiles("*.jpg");
        }
      }
      if (files != null && files.Count() > 0) {
        // problème de mémoire sur une grande quantité d'images
        //imgListLeft = new ImageList();
        //imgListLeft.Images.Clear();
        //var toto = files.Select(img => System.Drawing.Image.FromFile(img.FullName)).ToArray();
        //imgListLeft.Images.AddRange(toto);
        //var titi = imgListLeft.Images[imgListLeftIndex];
        pictureData.PathList     = files.Select(img => img.FullName).ToArray();
        pictureData.CurrentIndex = 0;
      } else {
        pictureData.PathList     = null;
        pictureData.CurrentIndex = -1;
      }
    }

    private void btnUploadLeft_Click(object sender, RoutedEventArgs e)
    {
      txtBoxFolderLeft.Text = BrowseFolder(txtBoxFolderLeft.Text);
    }

    private void btnUploadRight_Click(object sender, RoutedEventArgs e)
    {
      txtBoxFolderRight.Text = BrowseFolder(txtBoxFolderRight.Text);
    }

    private void txtBoxFolderLeft_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateImage((TextBox)sender, textBlkLeft, imgLeft, _pictureDataLeft);
    }

    private void txtBoxFolderRight_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateImage((TextBox)sender, textBlkLeft, imgRight, _pictureDataRight);
    }

    private void OnLeftPictureDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      UpdatePictureInfo(imgLeft, textBlkLeft, _pictureDataLeft.Filename);

      if (!string.IsNullOrEmpty(_pictureDataLeft.Filename)) {
        var fileInfo = new FileInfo(_pictureDataLeft.Filename);
        txtBlkFileLeft.Text = fileInfo.Name;
      }
    }

    private void OnRightPictureDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      UpdatePictureInfo(imgRight, textBlkRight, _pictureDataRight.Filename);

      if (!string.IsNullOrEmpty(_pictureDataRight.Filename)) {
        var fileInfo = new FileInfo(_pictureDataRight.Filename);
        txtBlkFileRight.Text = fileInfo.Name;
      }
    }

    private void btnImgLeftNext_Click(object sender, RoutedEventArgs e)
    {
      if (_pictureDataLeft.CurrentIndex < _pictureDataLeft.PathList.Count() - 1) {
        _pictureDataLeft.CurrentIndex++;
      }
    }

    private void btnImgLeftPrev_Click(object sender, RoutedEventArgs e)
    {
      if (_pictureDataLeft.CurrentIndex > 0) {
        _pictureDataLeft.CurrentIndex--;
      }
    }

    private void btnImgRightNext_Click(object sender, RoutedEventArgs e)
    {
      if (_pictureDataRight.CurrentIndex < _pictureDataRight.PathList.Count() - 1) {
        _pictureDataRight.CurrentIndex++;
      }
    }

    private void btnImgRightPrev_Click(object sender, RoutedEventArgs e)
    {
      if (_pictureDataRight.CurrentIndex > 0) {
        _pictureDataRight.CurrentIndex--;
      }
    }
  }
}
