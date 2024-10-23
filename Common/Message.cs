namespace Common
{
    public class Messager
    {
        public byte[] SendBuff;
        public int SendCount;
        public byte[] ReceiveBuff;
        public int ReceiveCount;

        public Messager()
        {
            SendBuff = new byte[1024];
            ReceiveBuff = new byte[1024];
        }

        public void WriteMessage(BaseMessage message)
        {
            int length = 1 + message.GetBytesLength();
            byte[] lengthBytes = BitConverter.GetBytes(length);
            Array.Copy(lengthBytes, 0, SendBuff, SendCount, lengthBytes.Length);
            SendCount += 4;
            SendBuff[SendCount] = (byte)message.MessageType;
            message.ToBytes(SendBuff, SendCount + 1); // TODO: handle when return false.
            SendCount += length;
        }

        public BaseMessage ReadMessage(int addWriteCount)
        {
            try
            {
                ReceiveCount += addWriteCount;
                if (ReceiveCount < 4)
                {
                    return null;
                }

                int length = BitConverter.ToInt32(ReceiveBuff, 0);
                if (ReceiveCount - 4 < length)
                {
                    return null;
                }

                MessageType messageType = (MessageType)ReceiveBuff[4];
                BaseMessage message = null;
                switch (messageType)
                {
                    case MessageType.String:
                        message = new StringMsg();
                        message.FromBytes(ReceiveBuff, 5, length - 1);
                        break;
                }
                Array.Copy(ReceiveBuff, length + 4, ReceiveBuff, 0, ReceiveCount - length - 4);
                ReceiveCount -= length + 4;

                return message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}