using System;
using System.Windows.Automation;
using System.Diagnostics;
using System.Threading;
namespace SMART.Demo
{
    public class SmartTest : IDisposable
    {
        Process process;
        AutomationElement mainwindow;

        public SmartTest()
        {
        }
        public event EventHandler DefectDetected;

        public virtual void startApp(params string[] parameters)
        {
            process = Process.Start(@"C:\Program Files (x86)\System Verification\Smart\Smart.exe");
            Thread.Sleep(5000);
        }

        public virtual void stopApp(params string[] parameters)
        {
            process.CloseMainWindow();
            process.Close();
        }

        public virtual void click(params string[] parameters)
        {
            if(mainwindow == null) throw new Exception("no mainwindow");
            
            Condition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, "NewProject");
            AutomationElement button;
            int count = 0;
            do
            {
                ++count;
                button = mainwindow.FindFirst(TreeScope.Children, condition);
                Thread.Sleep(100);
            } while (button == null && count < 10);

            

            if(button == null)
            {
                Debugger.Break();
                throw new Exception("could not find button with name : " + parameters[0]);
            }

            InvokePattern invokePattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokePattern.Invoke();

        }


        public virtual void OpenProjectWindow()
        {
            
            int count = 0;

            AutomationElementCollection windows;
            do
            {
                ++count;
                windows = AutomationElement.RootElement.FindAll(TreeScope.Children,
                                                                new PropertyCondition(
                                                                    AutomationElement.NameProperty, "SMART - System Verification AB"));
                Thread.Sleep(100);
            } while (windows.Count < 2 && count < 10);

            Condition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, "NewProject");
            AutomationElement button;
            count = 0;
            foreach (AutomationElement window in windows)
            {
                do
                {
                    ++count;
                    button = window.FindFirst(TreeScope.Children, condition);
                    Thread.Sleep(100);
                } while (button == null && count < 10);
                if(button != null)
                {
                    mainwindow = window;
                    break;
                }
            }

            if(mainwindow == null) throw new Exception("could not find root element MainWindow");

        }

        public virtual void ProjectOverviewWindow()
        {
            AutomationElement instance;
            int count = 0;
            do
            {
                ++count;
                instance = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                                                                   new PropertyCondition(
                                                                       AutomationElement.NameProperty, ""));
                Thread.Sleep(100);
            } while (instance == null && count < 10);
            
        }

        public void Reset()
        {
            process.CloseMainWindow();
            process.Close();
            mainwindow = null;
            Thread.Sleep(1000);

        }
        public void Dispose()
        {
            process.Close();            

        }
    }
}