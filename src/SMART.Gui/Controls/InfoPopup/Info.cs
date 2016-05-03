using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SMART.Gui.Properties;

namespace SMART.Gui.Controls.InfoPopup
{
    /// <summary>
    /// A markup extension that returns a
    /// <see cref="InfoPopup"/> control preconfigured
    /// with header and text information according to the
    /// <see cref="Title"/> and <see cref="Body"/>
    /// properties.
    /// </summary>
    public class Info : MarkupExtension
    {
        /// <summary>
        /// Either a title text or a resource key that can be used
        /// to look up the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Either a tooltips main text or a resource key that can be used
        /// to look up the text.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The image        
        /// </summary>
        public ImageSource InfoImage { get; set; }

        /// <summary>
        /// Empty default constructor.
        /// </summary>
        public Info()
        {
        }

        /// <summary>
        /// Inits the <see cref="Info"/> markup extension
        /// with title and body.
        /// </summary>
        public Info(string title, string body)
        {
            Title = title;
            Body = body;
        }

        /// <summary>
        /// Inits the <see cref="Info"/> markup extension
        /// with title, body and image.
        /// </summary>
        public Info(string title, string body, ImageSource image)
        {
            Title = title;
            Body = body;
            InfoImage = image;
        }

        /// <summary>
        /// Performs a lookup for the defined <see cref="Title"/> and
        /// <see cref="Info"/> and creates the tooltip control.
        /// </summary>
        /// <returns>
        /// The value of the resource that is specified by the
        /// <see cref="Title"/> property. If the property is not
        /// set, a null reference is returned.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //create the user control that 
            var popup = new InfoPopup();


            if (!String.IsNullOrEmpty(Title))
            {
                //look up title - if the string is not a
                //resource key, use it directly
                var result = Resources.ResourceManager.GetObject(Title) ?? Title;
                popup.HeaderText = (string)result;
            }

            if (!String.IsNullOrEmpty(Body))
            {
                //look up body text - if the string is not a
                //resource key, use it directly
                var result = Resources.ResourceManager.GetObject(Body) ?? Body;
                popup.BodyText = (string)result;
            }

            if (InfoImage != null)
            {
                popup.InfoImage = InfoImage;
            }

            //create tooltip and make sure it's not visible
            var tt = new ToolTip
                         {
                             HasDropShadow = false,
                             BorderThickness = new Thickness(0),
                             Background = Brushes.Transparent,
                             Content = popup
                         };

            return tt;
        }
    }
}
