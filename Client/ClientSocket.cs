using Common;
using GamePlay;
using GameSystem;
using System.Net.Sockets;

namespace Client
{
    public class ClientSocket
    {
        private static int s_HeartMsgInterval = 2000;

        private Socket m_Socket;
        private Messager m_Messager;

        public ClientSocket()
        {
            m_Socket = null;
            m_Messager = new Messager();
        }

        public void Connect(string ip, int port)
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            m_Socket.Connect(ip, port);
            Console.WriteLine("Connect success.");
            BeingReceive();

            ThreadPool.QueueUserWorkItem((obj) =>
            {
                while (m_Socket.Connected)
                {
                    BeingSend(new HeartMsg());
                    Thread.Sleep(s_HeartMsgInterval);
                }
            });
        }

        public void Close()
        {
            m_Socket.Shutdown(SocketShutdown.Both);
            m_Socket.Disconnect(false);
            m_Socket.Close();
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
            switch (message.MegId)
            {
                case 2002:
                    StringMsg stringMsg = message as StringMsg;
                    Console.WriteLine("Message '{0}' from '{1}'.", stringMsg.Str, m_Socket.RemoteEndPoint);
                    break;
            }
        }
    }
}
