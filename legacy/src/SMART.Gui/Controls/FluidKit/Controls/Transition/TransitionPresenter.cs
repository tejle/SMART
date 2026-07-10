// -------------------------------------------------------------------------------
// 
// This file is part of the FluidKit project: http://www.codeplex.com/fluidkit
// 
// Copyright (c) 2008, The FluidKit community 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this 
// list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice, this 
// list of conditions and the following disclaimer in the documentation and/or 
// other materials provided with the distribution.
// 
// * Neither the name of FluidKit nor the names of its contributors may be used to 
// endorse or promote products derived from this software without specific prior 
// written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
// -------------------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FluidKit.Controls
{
	public class TransitionPresenter : ItemsControl
	{
		private Grid _childContainer;
		private FrameworkElement _nextChild;
		private FrameworkElement _prevChild;
		private bool _isPlaying;
		private DispatcherTimer _restTimer;

		public static readonly DependencyProperty TransitionProperty;
		public static readonly DependencyProperty IsLoopedProperty;

		public bool IsLooped
		{
			get { return (bool)GetValue(IsLoopedProperty); }
			set { SetValue(IsLoopedProperty, value); }
		}

		public Transition Transition
		{
			get { return (Transition)GetValue(TransitionProperty); }
			set { SetValue(TransitionProperty, value); }
		}

		internal Grid TransitionContainer { get; private set; }
		public Duration RestDuration { get; set; }

		public event EventHandler TransitionCompleted;

		static TransitionPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionPresenter),
													 new FrameworkPropertyMetadata(
														typeof(TransitionPresenter)));
			TransitionProperty = DependencyProperty.Register("Transition",
									typeof(Transition), typeof(TransitionPresenter), new PropertyMetadata(null));
			IsLoopedProperty = DependencyProperty.Register("IsLooped",
				typeof(bool), typeof(TransitionPresenter), new PropertyMetadata(false, OnIsLoopedChanged));
		}

		private static void OnIsLoopedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			bool loop = (bool)e.NewValue;
			if (loop)
			{
				TransitionPresenter presenter = d as TransitionPresenter;
				presenter.Loaded += delegate
									{
										presenter.PlayLoopingTransition();
									};
			}
		}

		public void ApplyTransition(FrameworkElement prevChild, FrameworkElement nextChild)
		{
			if (_isPlaying) return;
			if (Transition == null)
			{
				throw new Exception("No Transition has been set");
			}

			_prevChild = prevChild;
			_nextChild = nextChild;
			_prevChild.Visibility = Visibility.Visible;
			_nextChild.Visibility = Visibility.Visible;

			// Switch to transition-mode
			SwitchToTransitionMode(true);
			Transition.Owner = this;
			Transition.Setup(CreateBrush(_prevChild), CreateBrush(_nextChild));

			Storyboard sb = Transition.PrepareStoryboard();
			PlayStoryboard(sb);
		}

		public void ApplyTransition(string prevChildName, string nextChildName)
		{
			// Check if the named children exist
			var prevChild = (FrameworkElement)FindName(prevChildName);
			var nextChild = (FrameworkElement)FindName(nextChildName);
			if (prevChild == null || nextChild == null) return;

			ApplyTransition(prevChild, nextChild);
		}

		public void AddTransitionElement(FrameworkElement elt)
		{
			TransitionContainer.Children.Add(elt);
		}

		public override void OnApplyTemplate()
		{
			_childContainer = GetTemplateChild("PART_ChildrenContainer") as Grid;
			TransitionContainer = GetTemplateChild("PART_TransitionElementsContainer") as Grid;
			base.OnApplyTemplate();
		}

		#region Core Methods
		private void PlayStoryboard(Storyboard sb)
		{
			EventHandler handler = null;
			handler = delegate
					{
						sb.Completed -= handler;
						FinishTransition();
					};
			sb.Completed += handler;
			sb.Begin(TransitionContainer);
		}

		private void SwitchToTransitionMode(bool isTransition)
		{
			if (isTransition)
			{
				TransitionContainer.Visibility = Visibility.Visible;
				_childContainer.Visibility = Visibility.Hidden;
			}
			else
			{
				TransitionContainer.Visibility = Visibility.Hidden;
				_childContainer.Visibility = Visibility.Visible;
			}

			_isPlaying = isTransition;
		}

		private static Brush CreateBrush(FrameworkElement elt)
		{
			VisualBrush brush = new VisualBrush(elt);
			brush.Viewbox = new Rect(0, 0, elt.ActualWidth, elt.ActualHeight);
			brush.ViewboxUnits = BrushMappingMode.Absolute;
			return brush;
		}

		private void ChangeChildrenStackOrder()
		{
			Panel.SetZIndex(_nextChild, 1);
			foreach (UIElement element in _childContainer.Children)
			{
				if (element != _nextChild)
				{
					Panel.SetZIndex(element, 0);
					element.Visibility = Visibility.Hidden;
				}
			}
		}

		public void FinishTransition()
		{
			Transition.Cleanup();
			TransitionContainer.Children.Clear();

			// Bring the next-child on top
			ChangeChildrenStackOrder();

			SwitchToTransitionMode(false);
			NotifyTransitionCompleted();
		}

		private void NotifyTransitionCompleted()
		{
			if (TransitionCompleted != null)
			{
				TransitionCompleted(this, null);
			}
		}

		#endregion

		private void PlayLoopingTransition()
		{
			// Need a transition and at least 2 children
			if (Transition == null || Items.Count < 2) return;

			int currentIndex = Items.Count-1;

			// Rest  between transitions
			_restTimer = new DispatcherTimer(DispatcherPriority.Normal);
			_restTimer.Interval = RestDuration.TimeSpan;
			_restTimer.Tick += delegate
                   {
					   _restTimer.Stop();

					   var prevChild = GetItem(currentIndex);

					   currentIndex = (currentIndex + 1) % Items.Count;
					   var nextChild = GetItem(currentIndex);

					   ApplyTransition(prevChild, nextChild);
				   };

			TransitionCompleted += delegate
				   {
					   _restTimer.Start();
				   };

			_restTimer.Start();
		}

		private FrameworkElement GetItem(int currentIndex)
		{
			return ItemContainerGenerator.ContainerFromIndex(currentIndex) as FrameworkElement;
		}
	}
}