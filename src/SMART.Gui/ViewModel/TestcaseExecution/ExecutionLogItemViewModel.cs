namespace SMART.Gui.ViewModel.TestcaseExecution
{
    using System;

    using Core.Interfaces;

    public enum LogLevel
    {
        Information,
        Warning,
        Error,
        Defect,
        Exception
    }
    public class ExecutionLogItemViewModel : ViewModelBase
    {
        private readonly string message;

        private readonly LogLevel level;

        public string Message { get { return message; } }

        public DateTime TimeStamp { get; private set; }

        public ExecutionLogItemViewModel(string message, LogLevel level)
        {
            this.message = message;
            this.level = level;

            TimeStamp = DateTime.Now;
        }

        public override string Icon
        {
            get
            {
                switch (level)
                {
                    case LogLevel.Information:
                        return Constants.LOG_INFORMATION_ICON;                        
                    case LogLevel.Warning:
                        return Constants.LOG_WARNING_ICON;
                    case LogLevel.Error:
                        return Constants.LOG_ERROR_ICON;
                    case LogLevel.Defect:
                        return Constants.LOG_DEFECT_ICON;
                    case LogLevel.Exception:
                        return Constants.LOG_EXCEPTION_ICON;
                    default:                        
                        break;
                }
                return Constants.MISSING_ICON_URL;
            }
        }

        public override Guid Id
        {
            get;
            set;
        }
    }
}
