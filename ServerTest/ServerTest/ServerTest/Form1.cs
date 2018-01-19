using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerTest
{
    struct ClientInfo
    {
        public EndPoint epPoint;
        public string name;    //客户端昵称
    }

    public partial class Form1 : Form
    {
        ArrayList clientList;   //存放客户端的endPoint
        Socket serverSocket;
        byte[] byteData = new byte[1024];
        byte[] by;

        public Form1()
        {
            clientList = new ArrayList();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            try
            {
                //创建socket，并绑定本地IP、port
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 60002);
                serverSocket.Bind(ipEndPoint);

                //开始接收client请求
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
                    SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (by != null)
            {
                foreach (ClientInfo info in clientList)
                {
                    SendTo(info.epPoint, by);
                }
            }
        }

        private void SendTo(EndPoint epSender, byte[] msg)
        {

            try
            {
                serverSocket.BeginSendTo(msg, 0,
                    msg.Length, SocketFlags.None,
                    epSender,
                    new AsyncCallback(OnSend),
                    null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                serverSocket.EndSendTo(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);
                //结束挂起的,从特定终结点进行异步读取
                int len = serverSocket.EndReceiveFrom(ar, ref epSender);
                //将某个客户端发来的数据转发给其他用户

                by = new byte[len];
                Array.Copy(byteData, 0, by, 0, len);
                ClientInfo ci = new ClientInfo();
                ci.epPoint = epSender;
                ci.name = "xx";
                if (!clientList.Contains(ci))
                {
                    clientList.Add(ci);
                }

                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None,
                    ref epSender, new AsyncCallback(OnReceive), epSender);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ServerUDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
