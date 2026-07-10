namespace SMART.Gui.Controls.DiagramControl.Helpers
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Converters;

    public static class BindingHelper
    {
        //public static MultiBinding CreateCenteredBinding(IConnectable source)
        //{
        //    // Create a multibinding collection and assign an appropriate converter to it
        //    MultiBinding multiBinding = new MultiBinding();
        //    multiBinding.Converter = new DefaultCenterBinding();
        //    multiBinding.ConverterParameter = new Point(
        //        ((FrameworkElement)source).ActualWidth / 2,
        //        ((FrameworkElement)source).ActualHeight / 2);

        //    // Create binging #1 to IConnectable to handle Left
        //    Binding binding = new Binding();
        //    binding.Source = source;
        //    binding.Path = new PropertyPath(Canvas.LeftProperty);
        //    multiBinding.Bindings.Add(binding);

        //    // Create binging #2 to IConnectable to handle Top
        //    binding = new Binding();
        //    binding.Source = source;
        //    binding.Path = new PropertyPath(Canvas.TopProperty);
        //    multiBinding.Bindings.Add(binding);

        //    return multiBinding;
        //}

        public static MultiBinding CreateAngledBinding(IConnectable source, IConnectable target)
        {
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Converter = new DefaultAngleCenterBinding();
            multiBinding.ConverterParameter = new Point(
                (((FrameworkElement)source).ActualWidth) / 2,                    
                (((FrameworkElement)source).ActualHeight) / 2);
            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath(Canvas.LeftProperty);
            binding.Mode = BindingMode.TwoWay;
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath(Canvas.TopProperty);
            binding.Mode = BindingMode.TwoWay;
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = target;
            binding.Path = new PropertyPath(Canvas.LeftProperty);
            binding.Mode = BindingMode.TwoWay;
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = target;
            binding.Path = new PropertyPath(Canvas.TopProperty);
            binding.Mode = BindingMode.TwoWay;
            multiBinding.Bindings.Add(binding);

            return multiBinding;
        }

        //  public static MultiBinding CreateBezierBinding(IConnectable source, IConnectable target)
        //  {
        //      MultiBinding multiBinding = new MultiBinding();
        //      multiBinding.Converter = new ConnectionSplineConverter();

        //      double[] sizes = new double[] 
        //{ 
        //    ((FrameworkElement)source).ActualWidth,
        //    ((FrameworkElement)source).ActualHeight,
        //    ((FrameworkElement)target).ActualWidth,
        //    ((FrameworkElement)target).ActualHeight
        //};

        //      bool reverse = false;

        //      if (Canvas.GetLeft((UIElement)source) < Canvas.GetLeft((UIElement)target))
        //      {
        //          reverse = true;
        //          multiBinding.ConverterParameter = new double[]
        //  {
        //    sizes[0],
        //    sizes[1] / 2,
        //    0,
        //    sizes[3] / 2
        //  };
        //      }
        //      else
        //      {
        //          reverse = false;
        //          multiBinding.ConverterParameter = new double[]
        //  {
        //    sizes[0],
        //    sizes[1] / 2,
        //    0,
        //    sizes[3] / 2
        //  };
        //      }

        //      Binding binding = new Binding();
        //      binding.Source = reverse ? source : target;
        //      binding.Path = new PropertyPath(Canvas.LeftProperty);
        //      multiBinding.Bindings.Add(binding);

        //      binding = new Binding();
        //      binding.Source = reverse ? source : target;
        //      binding.Path = new PropertyPath(Canvas.TopProperty);
        //      multiBinding.Bindings.Add(binding);

        //      binding = new Binding();
        //      binding.Source = reverse ? target : source;
        //      binding.Path = new PropertyPath(Canvas.LeftProperty);
        //      multiBinding.Bindings.Add(binding);

        //      binding = new Binding();
        //      binding.Source = reverse ? target : source;
        //      binding.Path = new PropertyPath(Canvas.TopProperty);
        //      multiBinding.Bindings.Add(binding);

        //      return multiBinding;
        //  }
    }
}