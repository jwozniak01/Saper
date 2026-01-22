using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Saper
{
    public enum ImageType
    {
        Blank,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Bomb,
        Flag,
        Mark,
        BlankGreen,
        RedBomb,
        BlueBomb
    }

    public class ImageManager
    {
        private ImageSource[] _images;

        public ImageManager()
        {
            LoadImages();
        }

        private void LoadImages()
        {
            _images = new ImageSource[15];
            _images[(int)ImageType.Blank] = new BitmapImage(new Uri("Images/blank.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.One] = new BitmapImage(new Uri("Images/1.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Two] = new BitmapImage(new Uri("Images/2.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Three] = new BitmapImage(new Uri("Images/3.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Four] = new BitmapImage(new Uri("Images/4.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Five] = new BitmapImage(new Uri("Images/5.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Six] = new BitmapImage(new Uri("Images/6.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Seven] = new BitmapImage(new Uri("Images/7.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Eight] = new BitmapImage(new Uri("Images/8.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Bomb] = new BitmapImage(new Uri("Images/bomb.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Flag] = new BitmapImage(new Uri("Images/flag.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.Mark] = new BitmapImage(new Uri("Images/mark.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.BlankGreen] = new BitmapImage(new Uri("Images/blankg.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.RedBomb] = new BitmapImage(new Uri("Images/redBomb.png", UriKind.RelativeOrAbsolute));
            _images[(int)ImageType.BlueBomb] = new BitmapImage(new Uri("Images/blueBomb.png", UriKind.RelativeOrAbsolute));
        }

        public Image GetImage(ImageType type)
        {
            Image image = new Image();
            image.Source = _images[(int)type];
            image.Stretch = Stretch.UniformToFill;
            return image;
        }

        public ImageSource GetImageSource(ImageType type)
        {
             return _images[(int)type];
        }
    }
}
