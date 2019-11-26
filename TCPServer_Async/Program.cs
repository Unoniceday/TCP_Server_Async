using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPServer_Async
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServerAsync();
            Console.ReadKey();
        }

        static Message msg = new Message();

        static void StartServerAsync()
        {
            //創建 Socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 綁定ip跟端口
            string myIp = "127.0.0.1"; //使用ipconfig來看自己的ip是哪個
            int port = 5566;    //選一個電腦內沒有被占用的port 

            IPAddress ipAddress = IPAddress.Parse(myIp);
            EndPoint point = new IPEndPoint(ipAddress, port);

            serverSocket.Bind(point);  //向系統申請可用的ip跟port 用來通信
            serverSocket.Listen(100);  //開始監聽，等待客戶端連接，最大連接數設為100

            //////////////////////////////////////////

            Console.WriteLine("server is running");
          
            serverSocket.BeginAccept(AccpetCallBack, serverSocket);

           

        }
        //接收數據到client
        static void AccpetCallBack(IAsyncResult ar)
        {
            
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);
            Console.WriteLine("client is connected");

            string s_msg = "Welcome !";
            byte[] data = Encoding.UTF8.GetBytes(s_msg); //將字串轉換成byte數組

            clientSocket.Send(data); //發送消息給clientSocket，這個時候就可以從client端看到從這個server傳過去的消息
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket); //跟Reveice差別在於BeginReceive是異步的

            //等待下一個客戶端連接
            serverSocket.BeginAccept(AccpetCallBack, serverSocket);

        }

        //接收client數據
        static byte[] dataBuffer = new byte[1024];
        static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket clientSocket = null;
            try
            {
                clientSocket = ar.AsyncState as Socket;
                int count = clientSocket.EndReceive(ar);

                if (count == 0)
                {
                    clientSocket.Close();
                    return;
                }
                msg.AddCount(count);
                //string msgStr = Encoding.UTF8.GetString(dataBuffer, 0, count);
                //Console.WriteLine("從客戶端收到數據 : " + msgStr);
                msg.ReadMessage();
                clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket); //跟Reveice差別在於BeginReceive是異步的
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (clientSocket != null)
                    clientSocket.Close();
            }
        }



        void StartServerSync()
        {
            //創建 Socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 綁定ip跟端口
            string myIp = "127.0.0.1"; //使用ipconfig來看自己的ip是哪個
            int port = 5566;    //選一個電腦內沒有被占用的port 

            IPAddress ipAddress = IPAddress.Parse(myIp);
            EndPoint point = new IPEndPoint(ipAddress, port);

            serverSocket.Bind(point);  //向系統申請可用的ip跟port 用來通信
            serverSocket.Listen(100);  //開始監聽，等待客戶端連接，最大連接數設為100

            //////////////////////////////////////////
            
            Console.WriteLine("server is running");
            Socket clientSocket = serverSocket.Accept();//暫停當前的線程，直到有一個客戶端連接過來，再進行下一步

            //ClientSocket client = new ClientSocket(clientSocket);
            //clientList.Add(client); //將連進來的客戶丟進列表李
            Console.WriteLine("client is connected");

            string s_msg = "Welcome !";
            byte[] data = Encoding.UTF8.GetBytes(s_msg); //將字串轉換成byte數組
            clientSocket.Send(data); //發送消息給clientSocket，這個時候就可以從client端看到從這個server傳過去的消息

            byte[] dataBuffer = new byte[1024];
            int count = clientSocket.Receive(dataBuffer);
            string msgReceive = System.Text.Encoding.UTF8.GetString(dataBuffer, 0, count);
            Console.WriteLine(msgReceive);

            Console.ReadKey();
            clientSocket.Close();
            serverSocket.Close();

        }
    }
}
