namespace Zoggr
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using ReplayTv;

    public class ZoggrHelper
    {
        private static string workingDir;

        public static string MakePathAndFile(string filename)
        {
            return (WorkingDir() + Path.DirectorySeparatorChar.ToString() + filename);
        }

        public static byte[] ReadFile(string filename)
        {
            byte[] array = null;
            ZoggrOptions options = new ZoggrOptions();
            options.LoadConfiguration();
            FileStream stream = null;
            try
            {
                stream = File.OpenRead(filename);
                array = new byte[(int)stream.Length];
                stream.Read(array, 0, (int)stream.Length);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Error reading {0}!", filename), exception);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return array;
        }

        public static void DeleteFile(string filename)
        {
            File.Delete(filename);
        }

        public static string replaceXMLSpecialChar(string toReplace)
        {
            if (toReplace != null)
            {
                toReplace = ReplayHelper.ReplaceUTF8InString(toReplace);
                toReplace = toReplace.Replace("&", "&amp;");
                toReplace = toReplace.Replace("<", "&lt;");
                toReplace = toReplace.Replace(">", "&gt;");
                toReplace = toReplace.Replace("\"", "&quot;");
                toReplace = toReplace.Replace("'", "&apos;");
                toReplace = toReplace.Replace("  ", " ");
            }
            return toReplace;
        }

        public static int UnixTime(DateTime fromTime)
        {
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
            return (int)(((ulong)(fromTime.Ticks - time.Ticks)) / 0x989680);
        }

        public static string WorkingDir()
        {
            if (workingDir == null)
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                workingDir = executingAssembly.Location.Substring(0, executingAssembly.Location.LastIndexOf(Path.DirectorySeparatorChar.ToString()));
            }
            return workingDir;
        }

        public static string WorkingDir(string pathname)
        {
            workingDir = pathname;
            return workingDir;
        }
    }
}
