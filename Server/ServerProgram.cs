namespace Server
{
    internal class ServerProgram
    {
        public static ServerSocket s_ServerSocket;

        static void Main(string[] args)
        {
            s_ServerSocket = new ServerSocket();
            s_ServerSocket.Start("127.0.0.1", 8080, 0);

            while (true)
            {
                s_ServerSocket.Update();

                string input = Console.ReadLine();
                if (input == "quit")
                {
                    s_ServerSocket.Close();
                    break;
                }
            }
        }
    }
}
