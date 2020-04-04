// Example for the Exception.GetBaseException method.
using CspExamples;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace NDP_UE_CS
{
    // Define two derived exceptions to demonstrate nested exceptions.
    class SecondLevelException : Exception
    {
        public SecondLevelException(string message, Exception inner)
            : base(message, inner)
        { }
    }
    class ThirdLevelException : Exception
    {
        public ThirdLevelException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    public class ArrayInstace
    {
        public int id { get; set; }
        public string Name { get; set; }
    }

    class NestedExceptions
    {
        public  void Main()
        {
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml("<book xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>" +
            //            "<title>Pride And Prejudice</title>" +
            //            "<title>Pride And Prejudice</title>" +                        
            //            "<title>Pride And Prejudice</title>" +
            //             "<title>Pride And Prejudice</title>" +
            //            "<title>Pride And Prejudice</title>" +
            //             "<title>Pride And Prejudice</title>" +
            //            "<title>Pride And Prejudice</title>" +
            //             "<title>Pride And Prejudice</title>" +
            //            "<title>Pride And Prejudice</title>" + "<title>Pride And Prejudice</title>" +
            //            "<title>Pride And Prejudice</title>" +
            //            "</book>");

            //string xsi = "http://www.w3.org/2001/XMLSchema-instance";
            //XmlElement root = doc.DocumentElement;

            

            //XmlNodeList xn = doc.GetElementsByTagName("title");
            //foreach (XmlNode n in xn)
            //{
            //    XmlAttribute newAttr = doc.CreateAttribute("nil", xsi);
            //    newAttr.Value = "true";
            //    n.InnerText = null;
            //    n.Attributes.Append(newAttr);
            //}
            

            //Console.WriteLine("Display the modified XML...");
            //Console.WriteLine(doc.InnerXml);





            //ArrayInstace[] arr = Array.CreateInstance(typeof(ArrayInstace), 2) as ArrayInstace[];
            //arr.SetValue(new ArrayInstace() { id = 1, Name = "text" }, 0);
            //arr.SetValue(new ArrayInstace() { id = 1, Name = "text" }, 1);

            //var check = arr.GetValue(1);

            //Array.Resize(ref arr, 10);

            //Console.WriteLine(arr.GetLowerBound(0));
            //string inputDate = "01-11-0001";
            //DateTime result;
            //if (DateTime.TryParse(inputDate, out result))
            //{
            //    DateTime compareDate;
            //    compareDate = new DateTime(1900, 01, 01);
            //    var dd = result.CompareTo(compareDate);
            //}
            // Define two DateTime objects for today's date 
            // next year and last year		


            try
            {
                // This function calls another that forces a 
                // division by 0.
                Rethrow();
                //throw new ArgumentOutOfRangeException("sample here");
            }
            catch (Exception exc)
            {
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                foreach (var r in Enumerable.Range(1, 1))
                {
                    ExceptionUtility.Error(exc);
                    ExceptionUtility.Error(exc.Message);
                    ExceptionUtility.Audit("example Audit details");
                    ExceptionUtility.Trace("example Trace details");
                    ExceptionUtility.Debug("example Debug details");
                    System.Threading.Thread.Sleep(1000);
                }

                Console.ReadLine();

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
#if DEBUG
                //System.Diagnostics.Debug.WriteLine("******exception*******");
                //System.Diagnostics.Debug.WriteLine(exc.ToString());
                //System.Diagnostics.Debug.WriteLine("******exception*******");

                //System.Diagnostics.Debug.WriteLine("");

                //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                //Console.WriteLine("RunTime " + elapsedTime);

                //string[] paths = { @"d:\archives", "2001", "media", "images" };
                //string fullPath = System.IO.Path.Combine(paths);
                //Console.WriteLine(fullPath);

                //string relative1 = "C:Documents";
                //ShowPathInfo(relative1);

                //string relative2 = "/Documents";
                //ShowPathInfo(relative2);

                //string absolute = "C:/Documents";
                //ShowPathInfo(absolute);

                //System.Diagnostics.Debug.WriteLine("RunTime " + elapsedTime);
#endif
            }

            Console.Write("Done");
            Console.ReadLine();
        }

        private static void ShowPathInfo(string path)
        {
            Console.WriteLine($"Path: {path}");
            Console.WriteLine($"   Rooted: {Path.IsPathRooted(path)}");
            //Console.WriteLine($"   Fully qualified: {Path.IsPathFullyQualified(path)}");
            Console.WriteLine($"   Full path: {Path.GetFullPath(path)}");
            Console.WriteLine();
        }

        // This function catches the exception from the called 
        // function DivideBy0( ) and throws another in response.
        static void Rethrow()
        {
            try
            {
                DivideBy0();
            }
            catch (Exception ex)
            {
                throw new ThirdLevelException(
                    "Caught the second exception and " +
                    "threw a third in response.", ex);
            }
        }

        // This function forces a division by 0 and throws a second 
        // exception.
        static void DivideBy0()
        {
            try
            {
                int zero = 0;
                int ecks = 1 / zero;
            }
            catch (Exception ex)
            {
                ex.Data["ExtraInfo"] = "Information from NestedRoutine1.";
                ex.Data.Add("MoreExtraInfo", "More information from NestedRoutine1.");
                throw new SecondLevelException(
                    "Forced a division by 0 and threw " +
                    "a second exception.", ex);
            }
        }
    }
}

/*
This example of Exception.GetBaseException generates the following output.

The program forces a division by 0, then throws the exception
twice more, using a different derived exception each time.

Unwind the nested exceptions using the InnerException property:

NDP_UE_CS.ThirdLevelException: Caught the second exception and threw a third in
 response. ---> NDP_UE_CS.SecondLevelException: Forced a division by 0 and thre
w a second exception. ---> System.DivideByZeroException: Attempted to divide by
 zero.
   at NDP_UE_CS.NestedExceptions.DivideBy0()
   --- End of inner exception stack trace ---
   at NDP_UE_CS.NestedExceptions.DivideBy0()
   at NDP_UE_CS.NestedExceptions.Rethrow()
   --- End of inner exception stack trace ---
   at NDP_UE_CS.NestedExceptions.Rethrow()
   at NDP_UE_CS.NestedExceptions.Main()

NDP_UE_CS.SecondLevelException: Forced a division by 0 and threw a second excep
tion. ---> System.DivideByZeroException: Attempted to divide by zero.
   at NDP_UE_CS.NestedExceptions.DivideBy0()
   --- End of inner exception stack trace ---
   at NDP_UE_CS.NestedExceptions.DivideBy0()
   at NDP_UE_CS.NestedExceptions.Rethrow()

System.DivideByZeroException: Attempted to divide by zero.
   at NDP_UE_CS.NestedExceptions.DivideBy0()

Display the base exception using the GetBaseException method:

System.DivideByZeroException: Attempted to divide by zero.
   at NDP_UE_CS.NestedExceptions.DivideBy0()
*/
