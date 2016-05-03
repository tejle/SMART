//using System.Windows.Input;
//using SMART.Gui;
//using Wpf;

//namespace ExtendedCommands
//{
//    static class GraphControlBoxCommandBindings
//    {
//        public static void IntializeCommandBindings(SmartGraphControl graphControl)
//        {
//            if (graphControl == null)
//                return;

//            graphControl.CommandBindings.Add(new CommandBinding(
//                                                SmartCommands.Zoom,
//                                                (sender, e) =>
//                                                {
//                                                    if (e.Parameter is double)
//                                                    {
//                                                        SetZoom(graphControl, (double)e.Parameter);
//                                                    }
//                                                },
//                                                (sender, e) =>
//                                                {
//                                                    var value = new RangedValue(GetZoom(graphControl), 0.01, 4);
//                                                    CommandUtil.SetCurrentValue(e, value);
//                                                    e.CanExecute = true;// TODO: (graphControl.Parent as SmartDockWindow).IsSelected;
//                                                    e.Handled = true;
//                                                }));

//            //graphControl.CommandBindings.Add(new CommandBinding(
//            //                        SmartCommands.AddVertex,
//            //                        (sender, e) =>
//            //                        {
//            //                            graphControl.CurrentItemTypeMode = ItemTypes.Vertex;
//            //                        },
//            //                        (sender, e) =>
//            //                        {
//            //                            e.CanExecute = true;
//            //                            e.Handled = false;
//            //                        }));

//            //graphControl.CommandBindings.Add(new CommandBinding(
//            //            SmartCommands.AddStart,
//            //            (sender, e) =>
//            //            {
//            //                graphControl.CurrentItemTypeMode = ItemTypes.Start;
//            //            },
//            //            (sender, e) =>
//            //            {
//            //                e.CanExecute = true;
//            //                e.Handled = false;
//            //            }));

//            //graphControl.CommandBindings.Add(new CommandBinding(
//            //            SmartCommands.AddStop,
//            //            (sender, e) =>
//            //            {
//            //                graphControl.CurrentItemTypeMode = ItemTypes.Stop;
//            //            },
//            //            (sender, e) =>
//            //            {
//            //                e.CanExecute = true;
//            //                e.Handled = false;
//            //            }));

//            //graphControl.CommandBindings.Add(new CommandBinding(
//            //            SmartCommands.AddComment,
//            //            (sender, e) =>
//            //            {
//            //                graphControl.CurrentItemTypeMode = ItemTypes.Comment;
//            //            },
//            //            (sender, e) =>
//            //            {
//            //                e.CanExecute = true;
//            //                e.Handled = false;
//            //            }));
//        }

//        private static double GetZoom(SmartGraphControl graphControl)
//        {
//            var value = graphControl.Zoom;
//            {
//                return (double)value;
//            }
//        }

//        private static void SetZoom(SmartGraphControl graphControl, double v)
//        {
//            graphControl.Zoom = v;
//        }

//    }
//}
