using Common;
using System.Net.Sockets;

namespace Server
{
    public class ClientSocket
    {
        private static int s_TimeOutTime = 10;

        private int m_ClientId;
        private Socket m_Socket;
        private Messager m_Messager;
        private long m_LastHeartMsgTime;

        public ClientSocket(int clientId, Socket socket)
        {
            m_ClientId = clientId;
            m_Socket = socket;
            m_Messager = new Messager();
            m_LastHeartMsgTime = -1;
        }

        public void Close()
        {
            m_Socket.Shutdown(SocketShutdown.Both);
            m_Socket.Close();
        }

        public bool CheckTimeOut()
        {
            if (m_LastHeartMsgTime != -1 && DateTime.Now.Ticks / TimeSpan.TicksPerSecond >= s_TimeOutTime)
            {
                return true;
            }

            return false;
        }

        public void BeingSend(BaseMessage message)
        {
            if (!m_Socket.Connected)
            {
                Console.WriteLine("Send failure. Connection has closed.");
                return;
            }

            m_Messager.WriteMessage(message);
            m_Socket.BeginSend(m_Messager.SendBuff, 0, m_Messager.SendCount, SocketFlags.None, SendCallback, null);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                m_Socket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to send with '{0}'.", e.ToString());
            }
        }

        public void BeingReceive()
        {
            m_Socket.BeginReceive(m_Messager.ReceiveBuff, m_Messager.ReceiveCount, m_Messager.ReceiveBuff.Length - m_Messager.ReceiveCount, SocketFlags.None, ReceiveCallback, null);
            Console.WriteLine("Start receiving from {0}.", m_Socket.RemoteEndPoint);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int count = m_Socket.EndReceive(ar);
            BaseMessage message = m_Messager.ReadMessage(count);
            if (message != null)
            {
                MessageHandler(message);
            }

            if (m_Socket.Connected)
            {
                m_Socket.BeginReceive(m_Messager.ReceiveBuff, m_Messager.ReceiveCount, m_Messager.ReceiveBuff.Length - m_Messager.ReceiveCount, SocketFlags.None, ReceiveCallback, null);
            }
            else
            {
                Console.WriteLine("Stop receive from '{0}'.", m_Socket.RemoteEndPoint);
            }
        }

        private void MessageHandler(BaseMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.String:
                    StringMsg stringMsg = message as StringMsg;
                    Console.WriteLine("Message '{0}' from '{1}'.", stringMsg.Str, m_Socket.RemoteEndPoint);
                    break;
                case MessageType.Heart:
                    m_LastHeartMsgTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                    break;
                case MessageType.Quit:
                    ServerProgram.s_ServerSocket.RemoveClient(this.m_ClientId);
                    break;
            }
        }
    }
}
