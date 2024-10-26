using System.Text;

namespace Common
{
    public abstract class IData
    {
        public abstract int GetByteCount();
        public abstract void Write(byte[] bytes, ref int index);
        public abstract void Read(byte[] bytes, ref int index);

        public static void WriteInt(byte[] bytes, ref int index, int value)
        {
            WriteBytes(bytes, ref index, BitConverter.GetBytes(value));
        }

        public static void WriteFloat(byte[] bytes, ref int index, float value)
        {
            WriteBytes(bytes, ref index, BitConverter.GetBytes(value));
        }

        public static void WriteBool(byte[] bytes, ref int index, bool value)
        {
            WriteBytes(bytes, ref index, BitConverter.GetBytes(value));
        }

        public static void WriteLong(byte[] bytes, ref int index, long value)
        {
            WriteBytes(bytes, ref index, BitConverter.GetBytes(value));
        }

        public static void WriteString(byte[] bytes, ref int index, string value)
        {
            WriteInt(bytes, ref index, value.Length);
            WriteBytes(bytes, ref index, Encoding.UTF8.GetBytes(value));
        }

        public static void WriteIData(byte[] bytes, ref int index, IData data)
        {
            data.Write(bytes, ref index);
        }

        public static void WriteBytes(byte[] bytes, ref int index, byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                bytes[index] = value[i];
                index += 1;
            }
        }

        public static int ReadInt(byte[] bytes, ref int index)
        {
            int value = BitConverter.ToInt32(bytes, index);
            index += 4;
            return value;
        }

        public static float ReadFloat(byte[] bytes, ref int index)
        {
            float value = BitConverter.ToSingle(bytes, index);
            index += 4;
            return value;
        }

        public static bool ReadBool(byte[] bytes, ref int index)
        {
            bool value = BitConverter.ToBoolean(bytes, index);
            index += 1;
            return value;
        }

        public static long ReadLong(byte[] bytes, ref int index)
        {
            long value = BitConverter.ToInt64(bytes, index);
            index += 4;
            return value;
        }

        public static string ReadString(byte[] bytes, ref int index)
        {
            int length = ReadInt(bytes, ref index);
            string value = Encoding.UTF8.GetString(bytes, index, length);
            index += length;
            return value;
        }

        public static T ReadIData<T>(byte[] bytes, ref int index) where T : IData, new()
        {
            T value = new T();
            value.Read(bytes, ref index);
            return value;
        }
    }
}