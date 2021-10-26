using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Shopper
{
    public class SearchCriteria
    {
        public static void ConfirmSearchCriteria()
        {
            var searchCriteria = CsvStorage.CheckForSearchCriteria();
            WriteLine($"Here is the search term: \"{searchCriteria.searchterm}\" and url: \"{searchCriteria.url}\"");
            WriteLine($"If you want a different search term, enter it in the next 10 seconds... ");
            try
            {
                string searchterm = ""; //Reader.ReadLine(10000);
                if (searchterm == "")
                {
                    searchterm = searchCriteria.searchterm;
                }
                WriteLine($"Thanks, you entered: \"{searchterm}\"");
                WriteLine($"Please enter the url to run searches on. You have 60 seconds... ");
                WriteLine($"If you want to use the same url, press the ENTER key");
                string url = "";//Reader.ReadLine(60000);
                if (url == "")
                {
                    CsvStorage.WriteSearchCriteriaFile(searchterm + "," + searchCriteria.url);
                }
                else
                {
                    CsvStorage.WriteSearchCriteriaFile(searchterm + "," + url);
                }
            }
            catch (TimeoutException)
            {
                WriteLine($"Proceding with running a search for {searchCriteria.searchterm}");
            }
        }
    }
    class Reader
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput, gotInput;
        private static string input;

        static Reader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }
        public static string ReadLine(int timeOutMillisecs = Timeout.Infinite)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                return input;
            else
                throw new TimeoutException();
        }
    }
}
