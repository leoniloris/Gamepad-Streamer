using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using SharpDX.DirectInput;

namespace client
{
    
    public partial class Client : Form
    {

        public ushort eita = 0;
        public Socket clientSocket { get; set; }
        public EndPoint epServer { get; set; }
        public EndPoint epSender;
        byte[] cntrl = new byte[10];
        bool test_with_virtual_driver = false;
        public bool ishost { get; set; } = false;
        public byte hostid { get; set; }
        gamepad_aq gp;
        public byte id { get; set; }
        public Client(){
            InitializeComponent();// controlador real
            if (test_with_virtual_driver)
                gp = new gamepad_aq();
            textBox1.Text="qw";

        }
        
        private void OnSend(IAsyncResult ar)
        {
            clientSocket.EndSend(ar);
            Thread.Sleep(50);// enviar o controle numa taxa de um pouco - de 20 controles/segundo
            ushort a;
            //if (test_with_virtual_driver) {
                a = gp.getState();
                cntrl[0] = (byte)'c'; cntrl[1] = (byte)'_'; cntrl[2] = (byte)id; cntrl[3] = hostid; cntrl[4] = (byte)a; cntrl[5] = (byte)(a >> 8);

            //}
            //else {
            //    eita++;
            //    try {
            //        cntrl[0] = (byte)'c'; cntrl[1] = (byte)'_'; cntrl[2] = (byte)id; cntrl[3] = (byte)textBox1.Text.ToCharArray()[0]; cntrl[4] = (byte)(textBox1.Text.ToCharArray()[0] >> 8);
            //    }catch {
            //        cntrl[0] = (byte)'c'; cntrl[1] = (byte)'_'; cntrl[2] = (byte)id; cntrl[3] = (byte)'0'; cntrl[4] = (byte)'0';
            //    }
            //}
            clientSocket.BeginSendTo(cntrl, 0, cntrl.Length, SocketFlags.None, epServer,new AsyncCallback(OnSend), null);
        }
        private void OnReceive(EndPoint epServer)    {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cntrl[0] = (byte)0; cntrl[1] = (byte)0; cntrl[2] = (byte)0; cntrl[3] = (byte)0;cntrl[4] = hostid;
            clientSocket.BeginSendTo(cntrl, 0, cntrl.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
        }
        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                clientSocket.SendTo(new byte[] { (byte)'q',(byte)'_',id,hostid,},epServer);
                clientSocket.Close();
            }
            catch (Exception) { }
            
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            clientSocket.SendTo(Encoding.ASCII.GetBytes(textBox1.Text),epServer);
        }
    }
    

}