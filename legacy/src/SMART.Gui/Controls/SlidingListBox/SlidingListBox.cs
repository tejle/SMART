// Copyright (C) Josh Smith - October 2006
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SMART.Gui.Controls.SlidingListBox
{
    /// <summary>
    /// Provides an animated slide effect when ListBoxItems are selected.
    /// </summary>
    public class SlidingListBox : ListBox
    {
        #region Data

        public static readonly DependencyProperty SlideDirectionProperty;
        public static readonly DependencyProperty SlideDistanceProperty;
        public static readonly DependencyProperty SlideDurationProperty;

        #endregion // Data

        #region Static Constructor

        static SlidingListBox()
        {
            SlideDirectionProperty =
                    DependencyProperty.Register( 
                            "SlideDirection", 
                            typeof( ListBoxItemSlideDirection ), 
                            typeof( SlidingListBox ), 
                            new UIPropertyMetadata( ListBoxItemSlideDirection.Right ) );

            SlideDistanceProperty =
                    DependencyProperty.Register( 
                            "SlideDistance", 
                            typeof( double ), 
                            typeof( SlidingListBox ),
                            new UIPropertyMetadata( 20.0, OnNumericSlidePropertyChanged<double> ) );

            SlideDurationProperty =
                    DependencyProperty.Register( 
                            "SlideDuration", 
                            typeof( int ), 
                            typeof( SlidingListBox ),
                            new UIPropertyMetadata( 200, OnNumericSlidePropertyChanged<int> ) );
        }

        #endregion // Static Constructor

        #region Public Properties

        /// <summary>
        /// Gets/sets the direction in which ListBoxItems are slid.  This is a dependency property.
        /// The default value is 'Right'.
        /// </summary>
        public ListBoxItemSlideDirection SlideDirection
        {
            get { return (ListBoxItemSlideDirection)this.GetValue( SlideDirectionProperty ); }
            set { this.SetValue( SlideDirectionProperty, value ); }
        }

        /// <summary>
        /// Gets/sets the number of logical pixels ListBoxItems are slid.  This is a dependency property.
        /// The default value is 20.
        /// </summary>
        public double SlideDistance
        {
            get { return (double)this.GetValue( SlideDistanceProperty ); }
            set { this.SetValue( SlideDistanceProperty, value ); }
        }

        /// <summary>
        /// Gets/sets the number of milliseconds the sliding animation takes for a ListBoxItems.  
        /// This is a dependency property. The default value is 200.
        /// </summary>
        public int SlideDuration
        {
            get { return (int)this.GetValue( SlideDurationProperty ); }
            set { this.SetValue( SlideDurationProperty, value ); }
        }

        #endregion // Public Properties

        #region OnNumericSlidePropertyChanged

        // Validates the value assigned to the SlideDistance and SlideDuration properties.
        static void OnNumericSlidePropertyChanged<T>( DependencyObject depObj, DependencyPropertyChangedEventArgs e ) 
                where T : IComparable
        {
            if( ((T)e.NewValue).CompareTo( default(T) ) < 0 )
                throw new ArgumentOutOfRangeException( e.Property.Name );
        }

        #endregion // OnNumericSlidePropertyChanged

        #region Animation Logic

        protected override void OnSelectionChanged( SelectionChangedEventArgs e )
        {
            base.OnSelectionChanged( e );

            var generator = this.ItemContainerGenerator;
            if( generator.Status != GeneratorStatus.ContainersGenerated )
                return;

            for( int i = 0; i < this.Items.Count; ++i )
            {
                var item = generator.ContainerFromIndex( i ) as ListBoxItem;
                if (item == null)
                    continue;
                if( VisualTreeHelper.GetChildrenCount( item ) == 0 )
                    continue;

                var rootBorder = VisualTreeHelper.GetChild( item, 0 ) as Border;
                if( rootBorder == null )
                    continue;

                this.AnimateItem( item, rootBorder );
            }           
        }

        void AnimateItem( ListBoxItem item, Border rootBorder )
        {
            // The default Left of a ListBoxItem's root Border's Padding
            // is 2, so the animation logic ensures that the Padding's Left
            // is always at 2 or the "slide distance".
            Thickness thickness;
            if( item.IsSelected )
            {
                ListBoxItemSlideDirection direction = this.SlideDirection;
                if( direction == ListBoxItemSlideDirection.Up )
                    thickness = new Thickness( 2, 0, 0, this.SlideDistance );
                else if( direction == ListBoxItemSlideDirection.Right )
                    thickness = new Thickness( 2 + this.SlideDistance, 0, 0, 0 );
                else if( direction == ListBoxItemSlideDirection.Down )
                    thickness = new Thickness( 2, this.SlideDistance, 0, 0 );
                else
                    thickness = new Thickness( 2, 0, this.SlideDistance, 0 );
            }
            else
            {
                thickness = new Thickness( 2, 0, 0, 0 );                    
            }

            TimeSpan timeSpan = TimeSpan.FromMilliseconds( this.SlideDuration );
            Duration duration = new Duration( timeSpan );
            ThicknessAnimation anim = new ThicknessAnimation( thickness, duration );
            rootBorder.BeginAnimation( Border.PaddingProperty, anim );
        }

        #endregion // Animation Logic
    }

    /// <summary>
    /// Represents the four directions in which a ListBoxItem can be slid.
    /// </summary>
    public enum ListBoxItemSlideDirection
    {
        Right,
        Left,
        Up,
        Down
    }
}