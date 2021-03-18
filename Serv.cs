using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PeerWebApp2
{
    public class Serv
    {
        private static byte[] MyKey = { 0x16, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x01 };
        private static byte[] IV = { 0x16, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x01 };
        Cryptos NewCrypt = new Cryptos(MyKey,IV);
        internal string serverRezult, serverCondition,otvet,rezult;
        


        public void Server(string myAdress, int myPort, Control lab1,Control lab2 )
        {
            IPEndPoint ipPointForServ = new IPEndPoint(IPAddress.Parse(myAdress), myPort);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {                
                listenSocket.Bind(ipPointForServ);
                listenSocket.Listen(3);//макс очередь из 3 входящих
                serverCondition = "Сервер запущен.Ждем писем...";
                lab2.Invoke((MethodInvoker)(() => lab2.Text = serverCondition));// по делегатом обновляем начинку контрола в его родительском потоке(через ссылку)

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    List<byte> arr = new List<byte>();
                    do
                    {
                        bytes = handler.Receive(data);
                        foreach (byte b in data)
                        {
                            arr.Add(b);
                        }
                    }
                    while (handler.Available > 0);

                    byte[] arrForDecrypt = new byte[arr.Count];
                    arrForDecrypt = arr.ToArray();
                    rezult = NewCrypt.DecryptStringFromBytes_Aes(arrForDecrypt);

                    serverRezult = (DateTime.Now.ToShortTimeString() + ":(fromClient) " + rezult);
                    lab1.Invoke((MethodInvoker)(()=> lab1.Text= serverRezult));//обновляем элемент в его родитесльком потоке, используя ссылку на элемент  

                    string message = "доставлено ";// отправляем ответ
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    // не закрываем сокет, чтоб сессия не прерывалась пока не закроется приложение
                }
            }
            catch (Exception ex)
            {
                serverRezult = ex.Message;
            }
        }


        public void Client(IPAddress addressForConnect, int portOpponent, string messageInput)
        {
            
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(addressForConnect, portOpponent);//адрес оппонента
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                socket.Connect(ipPoint);// подключаемся к удаленному хосту

                //byte[] data = Encoding.Unicode.GetBytes(messageInput);//тут можно ставить уже зашифрованный массив байтов
                byte[] data = NewCrypt.EncryptStringToBytes_Aes(messageInput);
                socket.Send(data);

                //получаем ответ
                data = new byte[128]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                //закрываем сокет, чтоб  можно было и дальше его юзать
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                otvet = DateTime.Now.ToShortTimeString() + ": "+ builder.ToString();//сохраняем ответ сервера(то что мы ему отправляли)
            }
            catch (Exception ex)
            {
                otvet = ex.Message;
            }

        }
    }

}
