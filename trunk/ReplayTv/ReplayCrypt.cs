namespace ReplayTv
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class ReplayCrypt
    {
        private static byte[] extraDataHTTPFS = new byte[] { 
            0xda, 0x76, 0x5c, 0xd4, 0x34, 0xc3, 0xd7, 0x2c, 0xac, 0x40, 0xb8, 0xd8, 0x59, 0xbc, 0x59, 0x34, 
            170, 0xbf, 0x89, 0xbd, 0x85, 0xe8, 0x40, 0x27, 120, 0x2b, 0x18, 110, 0xa6, 110, 90, 0xc6, 
            0xda, 0xe3, 0x86, 0x84, 0x40, 20, 0x2a, 0x23, 0x4f, 0x5d, 0x38, 0x5e, 0x7f, 0xd9, 0x73, 0x7d, 
            0xe4, 0x80, 0x3d, 0x21, 40, 0x41, 0xf1, 0xb2, 150, 0x43, 0x2b, 0xcc, 12, 0x9d, 0x26, 0xb9, 
            0, 0, 0, 0
         };
        private static byte[] extraDataRDDNS = new byte[] { 
            0x41, 0x47, 200, 9, 0xba, 60, 0x99, 0x6a, 0xda, 9, 0x9a, 15, 0xc0, 0xd3, 0x47, 0xca, 
            0xd1, 0x95, 0x81, 0x19, 0xab, 0x17, 0xc6, 0x5f, 0xad, 0xea, 0xe5, 0x75, 0x9c, 0x49, 0x18, 0xa5, 
            0xdf, 0x35, 70, 0x5b, 120, 14, 0xcb, 0xc7, 140, 0x3e, 0xf4, 0x90, 0xa2, 0xb7, 0x8e, 0, 
            0x53, 0x8d, 0x4c, 0xab, 0x13, 0xa5, 0x16, 0, 0xff, 0xb8, 0x4b, 0x20, 0x29, 0x22, 0x9d, 0xee, 
            0, 0, 0, 0
         };
        private static int timeOffset;
        private static DateTime UTCBase = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);

        public static byte[] HTTPFSDecrypt(byte[] cryptoText)
        {
            return RTVDecrypt(cryptoText, extraDataHTTPFS);
        }

        public static byte[] HTTPFSEncrypt(string plainText)
        {
            return RTVEncrypt(plainText, extraDataHTTPFS);
        }

        public static byte[] RDDNSDecrypt(byte[] cryptoText)
        {
            return RTVDecrypt(cryptoText, extraDataRDDNS);
        }

        public static byte[] RDDNSEncrypt(string plainText)
        {
            return RTVEncrypt(plainText, extraDataRDDNS);
        }

        private static void ReplayCryptBlock(ref byte[] cryptedData, uint key, int size, int offset)
        {
            for (int i = 0; i < size; i++)
            {
                key = (key * 0xb8f7) + 0x15bb9;
                cryptedData[i + offset] = (byte) (cryptedData[i + offset] ^ ((byte) key));
            }
        }

        private static byte[] RTVDecrypt(byte[] cryptoText, byte[] extraData)
        {
            MD5 md = new MD5CryptoServiceProvider();
            byte[] destinationArray = new byte[0x10];
            byte[] buffer2 = new byte[4];
            uint key = 0;
            byte[] buffer3 = new byte[4];
            byte[] buffer4 = new byte[4];
            byte[] buffer5 = new byte[(cryptoText.Length - 0x18) + extraData.Length];
            byte[] buffer6 = new byte[4];
            Array.Copy(cryptoText, 8, destinationArray, 0, 0x10);
            if (BitConverter.IsLittleEndian)
            {
                buffer2[0] = cryptoText[7];
                buffer2[1] = cryptoText[1];
                buffer2[2] = cryptoText[4];
                buffer2[3] = cryptoText[2];
            }
            else
            {
                buffer2[0] = cryptoText[2];
                buffer2[1] = cryptoText[4];
                buffer2[2] = cryptoText[1];
                buffer2[3] = cryptoText[7];
            }
            key = BitConverter.ToUInt32(buffer2, 0) ^ 0xcb0baf47;
            buffer4[0] = cryptoText[6];
            buffer4[1] = cryptoText[5];
            buffer4[2] = cryptoText[3];
            buffer4[3] = cryptoText[0];
            Array.Copy(cryptoText, 0x18, buffer5, 0, cryptoText.Length - 0x18);
            Array.Copy(extraData, 0, buffer5, cryptoText.Length - 0x18, extraData.Length);
            byte[] byteArray = md.ComputeHash(buffer5);
            if (!ReplayHelper.ByteArrayToHexString(byteArray).Equals(ReplayHelper.ByteArrayToHexString(destinationArray)))
            {
                object[] args = new object[] { ReplayHelper.ByteArrayToHexString(byteArray), ReplayHelper.ByteArrayToHexString(destinationArray) };
                ReplayLogger.DebugLog("Bad checksum (Computed: {0}) (Expected: {1})", args);
                return null;
            }
            ReplayCryptBlock(ref cryptoText, key, cryptoText.Length - 0x18, 0x18);
            Array.Copy(cryptoText, 0x18, buffer3, 0, 4);
            uint num2 = !BitConverter.IsLittleEndian ? 0x42ffdfa9 : 0xa9dfff42;
            if (BitConverter.ToUInt32(buffer3, 0) != num2)
            {
                object[] objArray2 = new object[] { ReplayHelper.ByteArrayToHexString(buffer3), BitConverter.ToInt32(buffer3, 0), 0x42ffdfa9 };
                ReplayLogger.DebugLog("Bad sanity number in decrypted data ({1} != {2}) {0}", objArray2);
                return null;
            }
            Array.Copy(cryptoText, 0x1c, buffer6, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer6);
            }
            int num3 = (int) (((ulong) (DateTime.UtcNow.Ticks - UTCBase.Ticks)) / 0x989680);
            timeOffset = ((int) BitConverter.ToUInt32(buffer6, 0)) - num3;
            byte[] buffer8 = new byte[cryptoText.Length - 0x20];
            Array.Copy(cryptoText, 0x20, buffer8, 0, cryptoText.Length - 0x20);
            return buffer8;
        }

        private static byte[] RTVEncrypt(string plainText, byte[] extraData)
        {
            MD5 md = new MD5CryptoServiceProvider();
            Random random = new Random();
            uint num = 0;
            uint num2 = 0;
            uint num3 = 0;
            byte[] destinationArray = new byte[0x20 + plainText.Length];
            byte[] buffer2 = new byte[(destinationArray.Length - 0x18) + extraData.Length];
            num = (uint) random.Next();
            num2 = (uint) random.Next();
            num3 = (uint) ((((ulong) (DateTime.UtcNow.Ticks - UTCBase.Ticks)) / 0x989680) + (ulong) timeOffset);
            num ^= 0xcb0baf47;
            byte[] array = BitConverter.GetBytes(num2);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(array);
            }
            byte[] bytes = BitConverter.GetBytes(num);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            destinationArray[0] = array[3];
            destinationArray[1] = bytes[2];
            destinationArray[2] = bytes[0];
            destinationArray[3] = array[2];
            destinationArray[4] = bytes[1];
            destinationArray[5] = array[1];
            destinationArray[6] = array[0];
            destinationArray[7] = bytes[3];
            num ^= 0xcb0baf47;
            destinationArray[0x18] = 0x42;
            destinationArray[0x19] = 0xff;
            destinationArray[0x1a] = 0xdf;
            destinationArray[0x1b] = 0xa9;
            byte[] buffer5 = BitConverter.GetBytes(num3);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer5);
            }
            destinationArray[0x1c] = buffer5[0];
            destinationArray[0x1d] = buffer5[1];
            destinationArray[30] = buffer5[2];
            destinationArray[0x1f] = buffer5[3];
            Array.Copy(Encoding.Default.GetBytes(plainText), 0, destinationArray, 0x20, plainText.Length);
            ReplayCryptBlock(ref destinationArray, num, destinationArray.Length - 0x18, 0x18);
            Array.Copy(destinationArray, 0x18, buffer2, 0, destinationArray.Length - 0x18);
            Array.Copy(extraData, 0, buffer2, destinationArray.Length - 0x18, extraData.Length);
            Array.Copy(md.ComputeHash(buffer2), 0, destinationArray, 8, 0x10);
            return destinationArray;
        }
    }
}
