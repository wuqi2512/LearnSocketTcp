using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ObjectiveC;

namespace Server
{
    public class ServerSocket
    {
        private Socket m_Socket;
        private int m_ClientId;
        private Dictionary<int, ClientSocket> m_ClientDic;
        private List<int> m_TempList;

        public ServerSocket()
        {
            m_Socket = null;
            m_ClientId = 0;
            m_ClientDic = new Dictionary<int, ClientSocket>();
            m_TempList = new List<int>();
        }

        public void Start(string ip, int port, int backlog)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.Bind(iPEndPoint);
            m_Socket.Listen(backlog);

            Console.WriteLine("Start waitting connect.");
            m_Socket.BeginAccept(AcceptCallback, m_Socket);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = m_Socket.EndAccept(ar);
                Console.WriteLine("{0} connected.", client.RemoteEndPoint);

                ClientSocket clientSocket = new ClientSocket(m_ClientId++, client);
                clientSocket.BeingReceive();
                lock (m_ClientDic)
                {
                    m_ClientDic.Add(m_ClientId, clientSocket);
                }

                m_Socket.BeginAccept(AcceptCallback, m_Socket);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Update()
        {
            lock (m_ClientDic)
            {
                foreach (var pair in m_ClientDic)
                {
                    if (pair.Value.CheckTimeOut())
                    {
                        m_TempList.Add(pair.Key);
                    }
                }
            }
            lock (m_TempList)
            {
                foreach (int id in m_TempList)
                {
                    lock (m_ClientDic)
                    {
                        if (m_ClientDic.TryGetValue(id, out var client))
                        {
                            client.Close();
                            m_ClientDic.Remove(id);
                        }
                    }
                }
                m_TempList.Clear();
            }
        }

        public void Close()
        {
            foreach (var client in m_ClientDic.Values)
            {
                client.Close();
            }
            m_ClientDic.Clear();

            m_Socket.Shutdown(SocketShutdown.Both);
            m_Socket.Close();
        }

        public void RemoveClient(int clientId)
        {
            lock (m_TempList)
            {
                m_TempList.Add(clientId);
            }
        }
    }
}
