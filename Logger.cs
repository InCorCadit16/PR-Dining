using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dining
{
    class Logger
    {

        public static void Log(string action)
        {
            Console.WriteLine($"{getPrefix()}: {action}");
        }

        private static string getPrefix()
        {
            return $"{DateTime.Now:HH:mm:ss:ffff} (Thread {Thread.CurrentThread.ManagedThreadId})";
        }
    }
}
