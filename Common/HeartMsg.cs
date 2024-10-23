namespace Common
{
    public class HeartMsg : BaseMessage
    {
        public override MessageType MessageType => MessageType.Heart;

        public override int GetBytesLength()
        {
            return 0;
        }

        public override bool ToBytes(byte[] bytes, int startIndex)
        {
            return true;
        }

        public override bool FromBytes(byte[] bytes, int startIndex, int length)
        {
            return true;
        }
    }
}
