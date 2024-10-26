using GamePlay;

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
            int length = 1 + message.GetByteCount();
            IData.WriteInt(SendBuff, ref SendCount, length);
            IData.WriteInt(SendBuff, ref SendCount, message.MegId);
            message.Write(SendBuff, ref SendCount);
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
                int index = 0;
                int length = IData.ReadInt(ReceiveBuff, ref index);
                if (ReceiveCount - 4 < length)
                {
                    return null;
                }

                
                int msgId = IData.ReadInt(ReceiveBuff, ref index);
                BaseMessage message = null;
                switch (msgId)
                {
                    case 2001: message = IData.ReadIData<PlayerMsg>(ReceiveBuff, ref index); break;
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