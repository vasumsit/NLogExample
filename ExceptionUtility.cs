using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspExamples
{
    public static class ExceptionUtility
    {
        public enum Names
        {
            Header,
            UserId,
            Message,
            Inner0exception,
            Base0exception,
            Full0exception
        }
        public enum NameOfExceptions
        {
            Type,
            Message,
            Custom0source,
            Inner0exception,
            Data,
            Help0link,
            Source,
            Stack0trace,
            Traget0site
        }

        public static Tuple<string, string, string, string, string> AskFormat(Exception ex)
        {
            StringBuilder formatMessage = new System.Text.StringBuilder();
            string expMessage = string.Empty;
            string innerMessage = string.Empty;

            formatMessage.Clear();
            foreach (var value in Enum.GetValues(typeof(Names)))
            {
                switch ((int)value)
                {
                    case (int)Names.Message:
                        expMessage = Exception(ex, formatMessage);
                        break;
                    case (int)Names.Inner0exception:
                        innerMessage = InnerException(ex, formatMessage);
                        break;
                    case (int)Names.Header:
                        formatMessage.AppendLine(NameWithValue(LogLevel.Error.Name, "", "h"));
                        break;
                    case (int)Names.Base0exception:
                        formatMessage.AppendLine(NameWithValue(ex.GetBaseException().ToString(), Enum.GetName(typeof(Names), (int)value)));
                        break;
                    case (int)Names.Full0exception:
                        formatMessage.AppendLine(NameWithValue(ex.ToString(), Enum.GetName(typeof(Names), (int)value)));
                        break;
                    case (int)Names.UserId:
                        formatMessage.AppendLine(NameWithValue("468714", Enum.GetName(typeof(Names), (int)value)));
                        break;
                    default:
                        formatMessage.AppendLine(NameWithValue("went wrong", Enum.GetName(typeof(Names), (int)value)));
                        break;
                }
            }
            return new Tuple<string, string, string, string, string>(formatMessage.ToString(), expMessage, innerMessage, ex.ToString(), ex.GetBaseException().ToString());
        }

        private static string NameWithValue(string value, string name, string section = "")
        {
            if (section == "h")
            {
                return $"*********************{DateTime.Now}:{value}:{Environment.MachineName}*************************";
            }
            else
            {
                return $"{name.Replace('0', ' ').PadRight(20)}:{value}";
            }
        }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void Error<T>(T exc) where T : Exception
        {
            // checking configuration before logging, configurable in logger section by level="Off"
            if (!logger.IsErrorEnabled)
                return;

            var result = AskFormat(exc);

            // creating parameters to pass exception to service
            // make sure pass type key as "sqlexeption" to identify and store details in exception table.         
            logger.Properties["type"] = "sqlexeption";
            logger.Properties["logmessage"] = result.Item1;
            logger.Properties["Exception"] = result.Item2 + result.Item4 + result.Item5;
            logger.Properties["StackTrace"] = exc.StackTrace.ToString();
            logger.Properties["InnerException"] = result.Item3;

            // fire level            
            logger.Error("");

            logger.Properties.Clear();

            // fire level            
            // logger.Error("{result}{evId}", result,"100");

            // log details in database
            // DBLog.Error(exc);
        }

        public static void Error(string formatedException)
        {
            // if disable return, configurable in logger section by level="Off"
            if (!logger.IsErrorEnabled)
                return;

            if (string.IsNullOrWhiteSpace(formatedException)) return;

            // creating StringBuilder instance to format exception
            StringBuilder formatMessage = new System.Text.StringBuilder();

            // Header
            formatMessage.AppendLine($"*********************{DateTime.Now}:{LogLevel.Error}*************************");
            formatMessage.Append(formatedException);

            // fire level
            // please make sure must pass same value to identify and write exception database in wcf service.
            logger.Properties["type"] = "sqlexeption";
            logger.Properties["Exception"] = formatMessage.ToString();
            logger.Properties["StackTrace"] = "";
            logger.Properties["InnerException"] = "";
            logger.Error(formatMessage);

            // clear exception
            formatMessage.Clear();
        }

        public static string Exception(Exception exc, StringBuilder formatMessage, string customMessage = "")
        {
            foreach (var value in Enum.GetValues(typeof(NameOfExceptions)))
            {
                switch ((int)value)
                {
                    case (int)NameOfExceptions.Message:
                        formatMessage.AppendLine(NameWithValue(exc.Message, Enum.GetName(typeof(NameOfExceptions), (int)value)));
                        break;
                    case (int)NameOfExceptions.Custom0source:
                        formatMessage.AppendLine(NameWithValue(customMessage, Enum.GetName(typeof(NameOfExceptions), (int)value)));
                        break;
                    case (int)NameOfExceptions.Data:
                        if (exc.Data.Count > 0)
                        {
                            formatMessage.AppendLine($"Extra Details:");
                            foreach (System.Collections.DictionaryEntry de in exc.Data)
                                formatMessage.AppendLine($" Key: {de.Key.ToString(),-20} Value: {de.Value}");
                        }
                        break;
                    case (int)NameOfExceptions.Help0link:
                        formatMessage.AppendLine(NameWithValue(exc.HelpLink?.ToString(), Enum.GetName(typeof(NameOfExceptions), (int)value)));
                        break;
                    case (int)NameOfExceptions.Source:
                        formatMessage.AppendLine(NameWithValue(exc.Source, Enum.GetName(typeof(NameOfExceptions), (int)value)));
                        break;
                    case (int)NameOfExceptions.Stack0trace:
                        StackTrace(exc, formatMessage);
                        break;
                    case (int)NameOfExceptions.Traget0site:
                        formatMessage.AppendLine(NameWithValue(exc.TargetSite?.ToString(), Enum.GetName(typeof(NameOfExceptions), (int)value)));
                        break;
                    case (int)NameOfExceptions.Type:
                        formatMessage.AppendLine(NameWithValue(exc.GetType().ToString(), Enum.GetName(typeof(NameOfExceptions), (int)value)));
                        break;
                    default:
                        formatMessage.AppendLine(NameWithValue("went wrong", Enum.GetName(typeof(NameOfExceptions), (int)value)));
                        break;
                }
            }

            // return to caller
            return formatMessage.ToString();
        }

        public static string StackTrace(Exception exc, StringBuilder formatMessage)
        {
            formatMessage.AppendLine("Stack Trace: ");
            if (exc.StackTrace != null)
            {
                formatMessage.AppendLine(exc.StackTrace);
            }
            return formatMessage.ToString();
        }

        public static string InnerException(Exception exc, StringBuilder formatMessage)
        {
            Exception Inner = exc.InnerException;
            if (Inner != null)
            {
                formatMessage.AppendLine($"****Inner Exception Begin****");
                while (Inner != null)
                {
                    Exception(Inner, formatMessage);
                    Inner = Inner.InnerException;
                }
                formatMessage.AppendLine($"****Inner Exception END****");
            }
            // return to caller
            return formatMessage.ToString();
        }

        public static void Audit(string message)
        {
            // if disable return, configurable in logger section by level="Off"
            if (!logger.IsInfoEnabled)
                return;
            if (string.IsNullOrWhiteSpace(message)) return;

            // log details in database
            DBLog.Audit(message);
        }

        public static void Debug(string message)
        {
            // if disable return, configurable in logger section by level="Off"
            if (!logger.IsDebugEnabled)
                return;

            if (string.IsNullOrWhiteSpace(message)) return;

            System.Text.StringBuilder formatMessage = new System.Text.StringBuilder();
            formatMessage.AppendLine($"*********************{DateTime.Now}:{LogLevel.Debug}*************************");
            formatMessage.Append(message);

            // trigger level
            logger.Debug(formatMessage);
            formatMessage.Clear();
        }

        public static void Trace(string message)
        {
            // if disable return, configurable in logger section by level="Off"
            if (!logger.IsTraceEnabled)
                return;

            if (string.IsNullOrWhiteSpace(message)) return;

            System.Text.StringBuilder formatMessage = new System.Text.StringBuilder();
            formatMessage.AppendLine($"*********************{DateTime.Now}:{LogLevel.Trace}*************************");
            formatMessage.Append(message);
            // trigger level
            logger.Debug(formatMessage);
            formatMessage.Clear();
        }
    }

    /// <summary>
    /// class to write exception and audit details in sql 
    /// class name must match nlog logger names like below 
    /// <logger name = "*.DBLog" minlevel="Error"  writeTo="testing" />
    /// <logger name = "*.DBLog" minlevel="Info" maxlevel="Info" final="true"  writeTo="testing" />
    /// </summary>
    public static class DBLog
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// to log exception details in Exception table(SLLog database)
        /// </summary>
        /// <param name="exc"></param>
        public static void Error(Exception exc)
        {
            // creating StringBuilder instance to format exception
            StringBuilder formatMessage = new System.Text.StringBuilder();

            formatMessage.AppendLine();
            formatMessage.AppendLine($"*********************{DateTime.Now}:{LogLevel.Error}:{Environment.MachineName} *************************");
            formatMessage.Append("Identity Type: ");
            formatMessage.AppendLine("");
            formatMessage.AppendLine("User: " + "");
            formatMessage.AppendLine("Custom Source: " + "customMessage");

            // creating parameters to pass exception to service
            // make sure pass type key as "sqlexeption" to identify and store details in exception table.         
            logger.Properties["type"] = "sqlexeption";
            logger.Properties["Exception"] = formatMessage.Append(ExceptionUtility.Exception(exc, new StringBuilder()));
            logger.Properties["StackTrace"] = ExceptionUtility.StackTrace(exc, new StringBuilder());
            logger.Properties["InnerException"] = ExceptionUtility.InnerException(exc, new StringBuilder());

            // fire level            
            logger.Error("");

            logger.Properties.Clear();
            formatMessage.Clear();
        }

        /// <summary>
        /// to log audit details in Audit table (SLLog database)
        /// </summary>
        /// <param name="message"></param>
        public static void Audit(string message)
        {
            System.Text.StringBuilder formatMessage = new System.Text.StringBuilder();
            formatMessage.AppendLine();
            formatMessage.AppendLine($"*********************{DateTime.Now}:{LogLevel.Info}*************************");
            formatMessage.AppendLine(message);

            // creating parameters to send values to service
            // passing key as "sqlaudit" to write in audit table-- DONT CHANGE
            logger.Properties["type"] = "sqlaudit";

            // trigger level
            logger.Info(formatMessage);

            logger.Properties.Clear();
            formatMessage.Clear();
        }
    }

    public class CustomeExcept : Exception
    {
        public CustomeExcept(Exception innerException, string message) : base(message, innerException)
        {

        }
    }

}
