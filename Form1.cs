using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace PeerWebApp2._0
{
    public partial class Form1 : Form
    {
        Serv NewServ = new Serv();
        ConnectInfo InfoForConnect = new ConnectInfo();
        string myAdress, FriendAdress;
        int myPort, FriendPort;
        private byte[] MyKey = { 0x16, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x01 };

        public Form1()
        {
            InitializeComponent();
            myAdress = InfoForConnect.GetMyAdress();//получаем свой динамический адрес
            myPort = 8005;
            FriendPort = 8005;
        }

        private void taskOne()
        {
            //передать ключ дешифровки
            NewServ.Server(myAdress, myPort,label1, label2 ); 
        }
        private void ServerStart(object sender, EventArgs e)
        {
            button2.Visible = false;
            Task serv = new Task(taskOne);
            serv.Start();
        }

        private void SendMessage(object sender, EventArgs e)
        {
            //юзать ключ шифровки
            NewServ.Client(IPAddress.Parse(textBox2.Text), FriendPort, textBox1.Text);
            label2.Text = NewServ.otvet;
            label1.Text += "\n" + DateTime.Now.ToShortTimeString() + ": " + textBox1.Text;
            textBox1.Text = "";
        }
    }
}
  