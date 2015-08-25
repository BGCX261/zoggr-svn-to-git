namespace ReplayTv
{
    using System;
    using System.IO;

    public class ReplayLogger
    {
        private static bool console_mode;
        private static bool debug_mode;
        private static string fileName = ReplayHelper.MakePathAndFile("replaytv.log");

        public static bool ConsoleMode()
        {
            return console_mode;
        }

        public static bool ConsoleMode(bool flag)
        {
            console_mode = flag;
            return console_mode;
        }

        public static void DebugLog(string entry)
        {
            if (debug_mode)
            {
                Log("DEBUG: " + entry);
            }
        }

        public static void DebugLog(string format, params object[] args)
        {
            if (debug_mode)
            {
                Log("DEBUG: " + string.Format(format, args));
            }
        }

        public static bool DebugMode()
        {
            return debug_mode;
        }

        public static bool DebugMode(bool flag)
        {
            debug_mode = flag;
            return debug_mode;
        }

        public static void Log(string entry)
        {
            if (console_mode)
            {
                Console.WriteLine(entry);
            }
            StreamWriter writer = null;
            try
            {
                FileInfo info = new FileInfo(fileName);
                if (info.Exists && (info.Length >= 0x100000))
                {
                    string path = fileName + ".old";
                    File.Delete(path);
                    info.MoveTo(path);
                }
                writer = new StreamWriter(fileName, true);
                writer.WriteLine("[{0:yyyy-MM-dd HH:mm:ss}] {1}", DateTime.Now, entry);
                writer.Flush();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        public static void LogThanks()
        {
            Log("Thanks: j.m., kjac, LeeThompson, TheDreamer, Xeriph, Darth Ed, StanLee, and the rest of the poopli people.");
        }
    }
}
