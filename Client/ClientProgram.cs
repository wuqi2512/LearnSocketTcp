using Common;

namespace Client
{
    internal class ClientProgram
    {
        static void Main(string[] args)
        {
            ClientSocket clientSocket = new ClientSocket();
            clientSocket.Connect("127.0.0.1", 8080);

            while (true)
            {
                string input = Console.ReadLine();
                if (input == null)
                {
                    continue;
                }
                else if (input == "quit")
                {
                    clientSocket.Close();
                    break;
                }
                else
                {
                    clientSocket.BeingSend(new StringMsg(input));
                }
            }
        }
    }
}
