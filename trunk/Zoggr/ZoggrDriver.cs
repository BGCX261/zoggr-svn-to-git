namespace Zoggr
{
    using System;
    using System.IO;

    internal class ZoggrDriver
    {
        public static bool noGetGuide;
        public static bool noRefreshGuide;
        public static bool noUpload;

        private static int Main(string[] args)
        {
            bool flag = false;
            bool offlineMode = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            string text = null;
            string text2 = null;
            string pathname = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar.ToString() + "Zoggr";
            for (int i = 0; i < args.Length; i++)
            {
                string str = args[i];
                if (str != null)
                {
                    str = string.IsInterned(str);
                    if (str == "--auto-add")
                    {
                        flag = true;
                        goto Label_0246;
                    }
                    if (str == "--add")
                    {
                        if ((i + 1) < args.Length)
                        {
                            text = args[++i].Trim();
                        }
                        goto Label_0246;
                    }
                    if (str == "--remove")
                    {
                        if ((i + 1) < args.Length)
                        {
                            text2 = args[++i].Trim();
                        }
                        goto Label_0246;
                    }
                    if (str == "--config")
                    {
                        if ((i + 1) < args.Length)
                        {
                            pathname = args[++i].Trim();
                        }
                        goto Label_0246;
                    }
                    if (str == "--debug")
                    {
                        flag4 = true;
                        goto Label_0246;
                    }
                    if (str == "--console")
                    {
                        flag5 = true;
                        goto Label_0246;
                    }
                    if (str == "--noupdate")
                    {
                        offlineMode = true;
                        goto Label_0246;
                    }
                    if (str == "--version")
                    {
                        Console.WriteLine(AppInfo.GetTitle() + " v" + AppInfo.GetVersion() + " - (c) 2008 Zoggr");
                        return 0;
                    }
                    if (str == "--usage")
                    {
                        flag3 = true;
                        goto Label_0246;
                    }
                    if (str == "--help")
                    {
                        flag3 = true;
                        goto Label_0246;
                    }
                    if (str == "--norefresh")
                    {
                        noRefreshGuide = true;
                        noUpload = true;
                        goto Label_0246;
                    }
                    if (str == "--norefreshguide")
                    {
                        noRefreshGuide = true;
                        noUpload = true;
                        goto Label_0246;
                    }
                    if (str == "--nogetguide")
                    {
                        noGetGuide = true;
                        noUpload = true;
                        goto Label_0246;
                    }
                }
                flag3 = true;
            Label_0246:
                if (flag3)
                {
                    break;
                }
            }
            if (flag3)
            {
                Console.Write((((((((AppInfo.GetTitle() + " v" + AppInfo.GetVersion() + " - (c) 2008 Zoggr\n") + "Options:\n") + "  --onvacation                 - Set vacation Mode on\n" + "  --novacation                 - Set vacation Mode off\n") + "  --auto-add\n" + "  --add <IP>\n") + "  --remove <RTV Serial Number>\n" + "  --config <dir>               - use <dir> instead of ~/.Zoggr\n") + "  --noupdate                   - don't update Zoggr\n" + "  --debug\n") + "  --console\n" + "  --version\n") + "  --usage\n" + "  --help\n");
                return 0;
            }
            ZoggrHelper.WorkingDir(pathname);
            if (!Directory.Exists(ZoggrHelper.WorkingDir()))
            {
                Directory.CreateDirectory(ZoggrHelper.WorkingDir());
                if (text == null)
                {
                    flag = true;
                }
            }
            ZoggrLogger.ConsoleMode(flag5);
            ZoggrLogger.DebugMode(flag4);
            if (offlineMode)
            {
                ZoggrLogger.Log("Working in offline mode");
            }
            new ZoggrUploader().Run(flag, text, text2, offlineMode);
            return 0;
        }
    }
}
