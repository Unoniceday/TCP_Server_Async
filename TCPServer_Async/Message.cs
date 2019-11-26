using System;
using System.Collections.Generic;
using System.Text;

namespace TCPServer_Async
{
    class Message
    {
        private byte[] data = new byte[1024]; //數據
        private int startIndex;//在這個數組裡存取了多少個byte

        public byte[] Data{ get { return data; }}

        public int StartIndex { get { return startIndex; } }

        public int RemainSize { get { return data.Length - startIndex; } }

        //增加數據
        public void AddCount(int count)
        {
            startIndex += count;
        }

        //讀取數據
        public void ReadMessage()
        {
            while (true)
            {
                if (startIndex <= 4) break;//小於4個字節不解析，等下次解析
                int count = BitConverter.ToInt32(data, 0);

                if ((startIndex - 4) >= count) //剩餘數據的長度
                {
                    string s = Encoding.UTF8.GetString(data, 4, count);
                    Console.WriteLine("解析出一條數據 : " + s);
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                    startIndex -= (count + 4);
                }
                else break;
            }
        }
    }
}
