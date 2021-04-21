using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureApp.View.BLEDevice.Extensions
{
    public class CodeUtils
    {
        #region shao
        // -----------------------------------------------------------------------------
        // DESCRIPTION: CRC-16校验的高位字节表
        // -----------------------------------------------------------------------------
        public static byte[] HiCRCTable = new byte[]{
            0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41, 0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40,
            0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40, 0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41,
            0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40, 0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41,
            0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41, 0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40,
            0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40, 0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41,
            0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41, 0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40,
            0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41, 0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40,
            0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40, 0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41,
            0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40, 0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41,
            0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41, 0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40,
            0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41, 0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40,
            0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40, 0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41,
            0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41, 0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40,
            0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40, 0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41,
            0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40, 0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41,
            0X00, 0XC1, 0X81, 0X40, 0X01, 0XC0, 0X80, 0X41, 0X01, 0XC0, 0X80, 0X41, 0X00, 0XC1, 0X81, 0X40
        };
        // -----------------------------------------------------------------------------
        // DESCRIPTION: CRC-16校验的低位字节表
        // -----------------------------------------------------------------------------
        public static byte[] LoCRCTable = new byte[]{
            0X00, 0XC0, 0XC1, 0X01, 0XC3, 0X03, 0X02, 0XC2, 0XC6, 0X06, 0X07, 0XC7, 0X05, 0XC5, 0XC4, 0X04,
            0XCC, 0X0C, 0X0D, 0XCD, 0X0F, 0XCF, 0XCE, 0X0E, 0X0A, 0XCA, 0XCB, 0X0B, 0XC9, 0X09, 0X08, 0XC8,
            0XD8, 0X18, 0X19, 0XD9, 0X1B, 0XDB, 0XDA, 0X1A, 0X1E, 0XDE, 0XDF, 0X1F, 0XDD, 0X1D, 0X1C, 0XDC,
            0X14, 0XD4, 0XD5, 0X15, 0XD7, 0X17, 0X16, 0XD6, 0XD2, 0X12, 0X13, 0XD3, 0X11, 0XD1, 0XD0, 0X10,
            0XF0, 0X30, 0X31, 0XF1, 0X33, 0XF3, 0XF2, 0X32, 0X36, 0XF6, 0XF7, 0X37, 0XF5, 0X35, 0X34, 0XF4,
            0X3C, 0XFC, 0XFD, 0X3D, 0XFF, 0X3F, 0X3E, 0XFE, 0XFA, 0X3A, 0X3B, 0XFB, 0X39, 0XF9, 0XF8, 0X38,
            0X28, 0XE8, 0XE9, 0X29, 0XEB, 0X2B, 0X2A, 0XEA, 0XEE, 0X2E, 0X2F, 0XEF, 0X2D, 0XED, 0XEC, 0X2C,
            0XE4, 0X24, 0X25, 0XE5, 0X27, 0XE7, 0XE6, 0X26, 0X22, 0XE2, 0XE3, 0X23, 0XE1, 0X21, 0X20, 0XE0,
            0XA0, 0X60, 0X61, 0XA1, 0X63, 0XA3, 0XA2, 0X62, 0X66, 0XA6, 0XA7, 0X67, 0XA5, 0X65, 0X64, 0XA4,
            0X6C, 0XAC, 0XAD, 0X6D, 0XAF, 0X6F, 0X6E, 0XAE, 0XAA, 0X6A, 0X6B, 0XAB, 0X69, 0XA9, 0XA8, 0X68,
            0X78, 0XB8, 0XB9, 0X79, 0XBB, 0X7B, 0X7A, 0XBA, 0XBE, 0X7E, 0X7F, 0XBF, 0X7D, 0XBD, 0XBC, 0X7C,
            0XB4, 0X74, 0X75, 0XB5, 0X77, 0XB7, 0XB6, 0X76, 0X72, 0XB2, 0XB3, 0X73, 0XB1, 0X71, 0X70, 0XB0,
            0X50, 0X90, 0X91, 0X51, 0X93, 0X53, 0X52, 0X92, 0X96, 0X56, 0X57, 0X97, 0X55, 0X95, 0X94, 0X54,
            0X9C, 0X5C, 0X5D, 0X9D, 0X5F, 0X9F, 0X9E, 0X5E, 0X5A, 0X9A, 0X9B, 0X5B, 0X99, 0X59, 0X58, 0X98,
            0X88, 0X48, 0X49, 0X89, 0X4B, 0X8B, 0X8A, 0X4A, 0X4E, 0X8E, 0X8F, 0X4F, 0X8D, 0X4D, 0X4C, 0X8C,
            0X44, 0X84, 0X85, 0X45, 0X87, 0X47, 0X46, 0X86, 0X82, 0X42, 0X43, 0X83, 0X41, 0X81, 0X80, 0X40
        };


        // *****************************************************************************
        // Design Notes:  
        // -----------------------------------------------------------------------------
        public static ushort QuickCRC16(string source, int startIndex, int lenght)
        {
            byte iHiVal;                // high byte of CRC initialized
            byte iLoVal;                // low byte of CRC initialized
            byte index;                 // will index into CRC lookup table
            uint i = 0;

            // Initial value for the CRC
            iHiVal = 0XFF;
            iLoVal = 0XFF;

            string dst = source.Substring(startIndex, lenght);
            byte[] byteDst = String2ByteArray(dst);
            int iSize = byteDst.Length;
            while (iSize != 0)
            {
                iSize--;
                // Calculate the CRC
                index = (byte)(iLoVal ^ byteDst[i++]);

                iLoVal = (byte)(iHiVal ^ HiCRCTable[index]);
                iHiVal = LoCRCTable[index];
            }
            return (ushort)(iHiVal << 8 | iLoVal);
        }

        public static float intStringToFloat(string data)
        {
            if (data.Length < 8 || data.Length > 8)
            {
                throw (new ApplicationException("缓存中的数据不完整！"));
            }
            else
            {
                byte[] intBuffer = new byte[4];
                //将16进制串按字节逆序化
                for (int i = 0; i < 4; i++)
                {
                    if ((i % 2) == 0)
                        intBuffer[i + 1] = Convert.ToByte(data.Substring(i * 2, 2), 16);
                    else
                        intBuffer[i - 1] = Convert.ToByte(data.Substring(i * 2, 2), 16);
                }
                return BitConverter.ToSingle(intBuffer, 0);
            }
        }

        #endregion

        public static string CRC16_AD(string source, int index, int lenght)
        {
            string dst = source.Substring(index, lenght);
            byte[] byteDst = String2ByteArray(dst);
            return String.Format("{0:X}", (int)CRC16_AD(byteDst));
        }

        public static uint CRC16_AD(byte[] source)
        {
            uint crc16 = 0xffff;
            for (uint i = 0; i < source.Length; i++)
            {
                crc16 ^= source[i];
                for (uint j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1)
                    {
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    }
                    else
                    {
                        crc16 = crc16 >> 1;
                    }
                }
            }
            return crc16;
        }

        public static string CRC16_Standard(string source, int index, int lenght)
        {
            string dst = source.Substring(index, lenght);
            byte[] byteDst = String2ByteArray(dst);
            return String.Format("{0:X}", (int)CRC16_Standard(byteDst));
        }

        public static uint CRC16_Standard(byte[] source)
        {
            uint crc16 = 0x0000;
            for (uint i = 0; i < source.Length; i++)
            {
                crc16 ^= source[i];
                for (uint j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1)
                    {
                        crc16 = (crc16 >> 1) ^ 0x8408;
                    }
                    else
                    {
                        crc16 = crc16 >> 1;
                    }
                }
            }
            return crc16;
        }

        public static uint CRC16_Modbus(byte[] modbusdata)
        {
            uint crc16 = 0xFFFF;
            for (uint i = 0; i < modbusdata.Length; i++)
            {
                crc16 ^= modbusdata[i];
                for (uint j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1)
                    {
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    }
                    else
                    {
                        crc16 = crc16 >> 1;
                    }
                }
            }
            return crc16;
        }

        public static byte[] getCrcByModBusData(byte[] modbusdata)
        {
            return BitConverter.GetBytes((short)CRC16_Modbus(modbusdata));
        }

        public static bool CRC16_validate(byte[] header, byte[] body)
        {
            byte[] arr = BitConverter.GetBytes((short)body.Length);
            byte[] len = new byte[] { arr[1], arr[0] };
            byte[] btArray = new byte[header.Length + len.Length + body.Length - 2];
            byte[] tail = new byte[2];
            Buffer.BlockCopy(header, 0, btArray, 0, header.Length);
            Buffer.BlockCopy(len, 0, btArray, header.Length, 2);
            Buffer.BlockCopy(body, 0, btArray, header.Length + len.Length, body.Length - 2);
            Buffer.BlockCopy(body, body.Length - 2, tail, 0, 2);
            byte[] crc = getCrcByModBusData(btArray);
            if (crc[0] != tail[0])
            {
                return false;
            }
            if (crc[1] != tail[1])
            {
                return false;
            }
            return true;
        }

        public static byte[] Time2BCD(string val)
        {
            byte[] bt = new byte[val.Length / 2];
            for (int i = 0; i < val.Length / 2; i++)
            {
                int ret = int.Parse(val.Substring(i * 2, 1)) * 16 + int.Parse(val.Substring(i * 2 + 1, 1));
                bt[i] = (byte)ret;
            }
            return bt;
        }

        public static byte String2BCD(string str)
        {
            return (byte)(int.Parse(str.Substring(0, 1)) * 16 + int.Parse(str.Substring(1, 1)));
        }

        public static byte[] String2ByteArray(string str)
        {
            byte[] ret = new byte[str.Length / 2];
            for (int i = 0; i < str.Length / 2; i++)
            {
                ret[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return ret;
        }

        public static byte String2Byte(string str)
        {
            return byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }

        public static DateTime String2DateTime(string str)
        {
            //20140911130956
            StringBuilder sb = new StringBuilder();
            sb.Append("20").Append(str.Substring(0, 2)).Append("-").
                Append(str.Substring(2, 2)).Append("-").
                Append(str.Substring(4, 2)).Append(" ").
                Append(str.Substring(6, 2)).Append(":").
                Append(str.Substring(8, 2)).Append(":").
                Append(str.Substring(10, 2));
            return Convert.ToDateTime(sb.ToString());
        }

        //输入为初始的9个字节的头加上指定的一串byte，输出为9个字节的头加上输入的byte数组
        public static byte[] createBytes(byte[] header, byte[] data)
        {
            byte[] setOut = new byte[data.Length + 9];

            Array.Copy(data, 0, setOut, 9, data.Length);

            setOut[0] = 0XAA;
            setOut[1] = 0X1D;
            setOut[2] = header[2];
            setOut[3] = header[3];
            setOut[4] = 0X03;
            setOut[5] = 0X00;
            setOut[6] = System.BitConverter.GetBytes(data.Length)[0];
            setOut[7] = 0X00;
            setOut[8] = 0X00;

            String crcIn = "";
            for (int i = 0; i < setOut.Length; i++)
            {
                crcIn += setOut[i].ToString("X2");
            }
            ushort crcOut = CodeUtils.QuickCRC16(crcIn, 0, crcIn.Length);
            byte[] crcOutByte = BitConverter.GetBytes(crcOut);
            setOut[7] = crcOutByte[1];
            setOut[8] = crcOutByte[0];

            return setOut;
        }

        #region
        //解码数据后，加入3:4编码
        public static byte[] adDecode(byte[] data)
        {
            int srcLen = data.Length;
            if (srcLen % 4 != 0)
            {
                return null;
            }
            int destLen = (data.Length / 4) * 3;
            byte[] dst = new byte[destLen];
            int j = 0;
            for (int i = 0; i < data.Length / 4; i++)
            {
                dst[j] = (byte)((data[i * 4 + 0] & 0x3f) | (data[i * 4 + 3] << 2 & 0xc0));
                dst[j + 1] = (byte)((data[i * 4 + 1] & 0x3f) | (data[i * 4 + 3] << 4 & 0xc0));
                dst[j + 2] = (byte)((data[i * 4 + 2] & 0x3f) | (data[i * 4 + 3] << 6 & 0xc0));
                j = j + 3;
            }
            String temp = BitConverter.ToString(dst);
            String[] temp0 = temp.Split('-');
            int len = Int16.Parse((temp0[2] + temp0[3]), System.Globalization.NumberStyles.HexNumber) + 6;

            byte[] result = new byte[len + 2];
            for (int i = 0; i < len; i++)
            {
                result[i] = dst[i];
            }
            result[len] = 0x0D;
            result[len + 1] = 0x0A;
            return result;
        }

        public static byte[] adEncode(byte[] data)
        {
            int len = data.Length % 3 != 0 ? ((data.Length / 3) + 1) * 3 : data.Length;
            byte[] extentData = new byte[len];
            data.CopyTo(extentData, 0);
            byte[] dst = new byte[(len / 3) * 4 + 2];
            int j = 0;

            for (int i = 0; i < len / 3; i++)
            {
                dst[j + 0] = (byte)(extentData[i * 3 + 0] & 0x3f | 0x40);
                dst[j + 1] = (byte)(extentData[i * 3 + 1] & 0x3f | 0x40);
                dst[j + 2] = (byte)(extentData[i * 3 + 2] & 0x3f | 0x40);
                dst[j + 3] = (byte)(0x40 | ((extentData[i * 3 + 0] >> 2) & 0x30)
                    | ((extentData[i * 3 + 1] >> 4) & 0x0c)
                    | ((extentData[i * 3 + 2] >> 6) & 0x03));
                j = j + 4;
            }
            dst[dst.Length - 2] = 0x0D;
            dst[dst.Length - 1] = 0x0A;
            return dst;
        }
        #endregion


        #region 雨量CRC校验
        //末尾增加CRC校验 CRC校验不包含帧尾
        public static byte[] yl_addCrc(Byte[] src)
        {
            byte[] result = new byte[src.Length];//初始化数组
            //进行效验的应该是长度减3
            Array.Copy(src, 0, result, 0, src.Length);
            String crcIn = "";
            for (int i = 0; i < src.Length - 3; i++)
            {
                crcIn += src[i].ToString("X2");
            }
            ushort crcOut = CodeUtils.QuickCRC16(crcIn, 0, crcIn.Length);
            byte[] crcOutByte = BitConverter.GetBytes(crcOut);
            result[13] = crcOutByte[1];
            result[14] = crcOutByte[0];
            return result;
        }
        #endregion
    }
}
