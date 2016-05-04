﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SMART.Gui.Controls.InfoPopup
{
    /// <summary>
    /// Interaction logic for InfoPopup.xaml
    /// </summary>
    public partial class InfoPopup
    {
        public InfoPopup()
        {
            InitializeComponent();
        }

        #region HeaderText dependency property

        /// <summary>
        /// An optional title to be displayed. May be omitted.
        /// </summary>
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText",
                                        typeof(string),
                                        typeof(InfoPopup),
                                        new FrameworkPropertyMetadata("", HeaderTextPropertyChanged));

        /// <summary>
        /// A property wrapper for the <see cref="HeaderTextProperty"/>
        /// dependency property:<br/>
        /// An optional title to be displayed. May be omitted.
        /// </summary>
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }


        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="HeaderTextProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnHeaderTextPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void HeaderTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = (InfoPopup)d;
            owner.OnHeaderTextPropertyChanged(e);
        }


        /// <summary>
        /// Handles changes of the <see cref="HeaderTextProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="HeaderText"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary
        /// <param name="e">Provides information about the updated property.</param>
        private void OnHeaderTextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var newValue = (string)e.NewValue;
            txtTitle.Visibility = String.IsNullOrEmpty(newValue)
                                      ? Visibility.Collapsed
                                      : Visibility.Visible;
        }

        #endregion

        #region BodyText dependency property

        /// <summary>
        /// The information to be displayed through the popup.
        /// </summary>
        public static readonly DependencyProperty BodyTextProperty =
            DependencyProperty.Register("BodyText",
                                        typeof(string),
                                        typeof(InfoPopup),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="BodyTextProperty"/>
        /// dependency property:<br/>
        /// The information to be displayed through the popup.
        /// </summary>
        public string BodyText
        {
            get { return (string)GetValue(BodyTextProperty); }
            set { SetValue(BodyTextProperty, value); }
        }

        #endregion

        #region Image dependency property

        /// <summary>
        /// The image to be displayed through the popup.
        /// </summary>
        public static readonly DependencyProperty InfoImageProperty =
            DependencyProperty.Register("InfoImage",
                                        typeof(ImageSource),
                                        typeof(InfoPopup),
                                        new FrameworkPropertyMetadata());

        /// <summary>
        /// A property wrapper for the <see cref="BodyTextProperty"/>
        /// dependency property:<br/>
        /// The information to be displayed through the popup.
        /// </summary>
        public ImageSource InfoImage
        {
            get { return (ImageSource)GetValue(InfoImageProperty); }
            set { SetValue(InfoImageProperty, value); }
        }

        #endregion
    }
}
