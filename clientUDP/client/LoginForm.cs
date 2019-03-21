using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace client
{
    public partial class LoginForm : Form
    {
        public Socket clientSocket;
        public Socket tcpsock;
        public EndPoint epServer;
        byte[] byteData;
        public byte id;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //UDP sockets
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //tcpsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string[] ip = "174.74.91.23".Split('.'); //(txtServerIP.Text.Split('.').ToArray()); //
            IPAddress ipip = new IPAddress(new byte[] { byte.Parse(ip[0]), byte.Parse(ip[1]), byte.Parse(ip[2]), byte.Parse(ip[3]) });
            IPEndPoint ipEndPoint = new IPEndPoint(ipip, 1000);//porta 1000
            epServer = (EndPoint)ipEndPoint; //ESTE ENDPOINT É DO SERVER MSM
            // criar data p cliente -> svr
            
                // login to the server
            //clientSocket.BeginSendTo(byteData, 0, byteData.Length,SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
            Hand_shake_client();

        }
        private void Hand_shake_client()
        {
            //id=(byte)numericUpDown1.Value;
            char[] a = { 'l', '_', (char)(uint)0,(char)(uint)hostid.Value };
            byteData = Encoding.ASCII.GetBytes(a);
            for(int q=1;q<100;q++)clientSocket.SendTo(byteData,epServer);
            byte aa = 1;
            while (aa==1) { 
                clientSocket.ReceiveFrom(byteData,ref epServer);
                var az = new string(Encoding.UTF8.GetString(byteData).ToCharArray());
                if (az.Contains("OK")) { DialogResult = DialogResult.OK; id = (byte)az.ToArray()[2]; aa = 0; Close();}
            }
        }
        private void btnCancel_Click(object sender, EventArgs e){Close();}
        private void LoginForm_Load(object sender, EventArgs e){CheckForIllegalCrossThreadCalls = false;}
        private void hostcb_CheckedChanged(object sender, EventArgs e)
        {
            hostid.Visible = false; label3.Visible = false;
        }
    }
}