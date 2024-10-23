using System.Text;

namespace Common
{
    public class StringMsg : BaseMessage
    {
        public string Str;

        public StringMsg()
        {

        }

        public StringMsg(string str)
        {
            Str = str;
        }

        public override MessageType MessageType => MessageType.String;

        public override int GetBytesLength()
        {
            return Encoding.UTF8.GetByteCount(Str);
        }

        public override bool ToBytes(byte[] bytes, int startIndex)
        {
            Encoding.UTF8.GetBytes(Str, 0, Str.Length, bytes, startIndex);
            return true;
        }

        public override bool FromBytes(byte[] bytes, int startIndex, int count)
        {
            Str = Encoding.UTF8.GetString(bytes, startIndex, count);
            return true;
        }
    }
}
