namespace Common
{
    public abstract class BaseMessage
    {
        public abstract MessageType MessageType { get; }
        public abstract int GetBytesLength();
        public abstract bool ToBytes(byte[] bytes, int startIndex);
        public abstract bool FromBytes(byte[] bytes, int startIndex, int length);
    }
}
