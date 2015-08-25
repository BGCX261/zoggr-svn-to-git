namespace ReplayTv
{
    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Globalization;

    public class ReplayHelper
    {
        private static string workingDir;

        public static string MakePathAndFile(string filename)
        {
            return (WorkingDir() + Path.DirectorySeparatorChar.ToString() + filename);
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


        public static string ByteArrayToHexString(byte[] byteArray)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteArray.Length; i++)
            {
                builder.Append(byteArray[i].ToString("X2"));
            }
            return builder.ToString();
        }

        public static int FindInByteArray(byte[] Haystack, byte[] Needle)
        {
            int num2 = -1;
            int index = 0;
            for (int i = 0; i < Haystack.Length; i++)
            {
                if (Haystack[i] == Needle[index])
                {
                    if (index == 0)
                    {
                        num2 = i;
                    }
                    if (index == (Needle.Length - 1))
                    {
                        return num2;
                    }
                    index++;
                }
                else
                {
                    num2 = -1;
                    index = 0;
                }
            }
            return -1;
        }

        public static byte[] GrowArray(byte[] array, int size)
        {
            byte[] destinationArray = new byte[array.Length + size];
            Array.Copy(array, destinationArray, array.Length);
            return destinationArray;
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            byte[] buffer = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                string s = hexString.Substring(i, 2);
                buffer[i / 2] = byte.Parse(s, NumberStyles.HexNumber);
            }
            return buffer;
        }

        public static byte[] ReplaceInByteArray(byte[] OriginalArray, byte[] Find, byte[] Replace)
        {
            byte[] array = OriginalArray;
            if (Array.BinarySearch(array, Find) > -1)
            {
                while (FindInByteArray(array, Find) > -1)
                {
                    byte[] buffer2 = new byte[(array.Length + Replace.Length) - Find.Length];
                    int num = FindInByteArray(array, Find);
                    int index = 0;
                    int num3 = 0;
                    for (int i = 0; i < num; i++)
                    {
                        buffer2[i] = array[i];
                        index++;
                        num3++;
                    }
                    for (int j = 0; j < Replace.Length; j++)
                    {
                        buffer2[index] = Replace[j];
                        index++;
                    }
                    for (num3 += Find.Length; index < buffer2.Length; num3++)
                    {
                        buffer2[index] = array[num3];
                        index++;
                    }
                    array = buffer2;
                }
            }
            return array;
        }

        public static string ReplaceUTF8InString(string SamIAm)
        {
            char[] chArray = "".ToCharArray();
            char[] chArray2 = "''',!cLoY|s\"\"\"ca<--r_o+23'uP.,10>123?AAAAAAACEEEEIIIIDNOOOOOx0UUUUYbBaaaaaaaceeeeiiiinoooooo/ouuuuyby".ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                if (SamIAm.IndexOf(chArray[i]) >= 0)
                {
                    SamIAm = SamIAm.Replace(chArray[i], chArray2[i]);
                }
            }
            return SamIAm;
        }

        public static int ReplayByteToInt(byte[] array, int offset, int length)
        {
            int num3;
            try
            {
                byte[] destinationArray = new byte[length];
                Array.Copy(array, offset, destinationArray, 0, length);
                int num = 0;
                for (int i = 0; i < length; i++)
                {
                    if ((i > 0) && (destinationArray[(length - i) - 1] > 0))
                    {
                        num += (int) (Math.Pow(2, (double) (8 * i)) * destinationArray[(length - i) - 1]);
                    }
                    else
                    {
                        num += destinationArray[(length - i) - 1];
                    }
                }
                num3 = num;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return num3;
        }

        public static long ReplayByteToLong(byte[] array, int offset, int length)
        {
            long num3;
            try
            {
                byte[] destinationArray = new byte[length];
                Array.Copy(array, offset, destinationArray, 0, length);
                long num = 0;
                for (int i = 0; i < length; i++)
                {
                    if ((i > 0) && (destinationArray[(length - i) - 1] > 0))
                    {
                        num += (long) (Math.Pow(2, (double) (8 * i)) * destinationArray[(length - i) - 1]);
                    }
                    else
                    {
                        num += destinationArray[(length - i) - 1];
                    }
                }
                num3 = num;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return num3;
        }

        public static byte[] ReplayIntToByte(int val, int size)
        {
            byte[] buffer = new byte[size];
            int index = size - 1;
            for (int i = 0; index >= 0; i++)
            {
                buffer[index] = (byte) ((val >> (i * 8)) & 0xff);
                index--;
            }
            return buffer;
        }
    }
}
