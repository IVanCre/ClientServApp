using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace PeerWebApp2._0
{
    class Serv
    {
        internal string serverRezult, serverCondition,otvet;

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
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    serverRezult = (DateTime.Now.ToShortTimeString() + ":(fromClient) " + builder.ToString());//при входящем сообщении
                                                                                                  
                    string message = "доставлено ";// отправляем ответ
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    lab1.Invoke((MethodInvoker)(()=> lab1.Text= serverRezult));//обновляем элемент в его родитесльком потоке, используя ссылку на элемент

                    // не закрываем сокет, чтоб сессия не прерывалась пока не закроется приложение
                    //handler.Shutdown(SocketShutdown.Both);
                    //handler.Close();
                }
            }
            catch (Exception ex)
            {
                serverRezult = ex.Message;
            }
        }


        public void Client(IPAddress addressForConnect, int portOpponent, string message)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(addressForConnect, portOpponent);//адрес оппонента
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                socket.Connect(ipPoint);// подключаемся к удаленному хосту

                string messageSend = message;
                byte[] data = Encoding.Unicode.GetBytes(messageSend);
                socket.Send(data);

                //получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);


                ////закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                otvet = DateTime.Now.ToShortTimeString() + ": "+ builder.ToString();//сохраняем ответ сервера( то что мы ему отправляли)
            }
            catch (Exception ex)
            {
                otvet= ex.Message;
            }
        }
    }

}
