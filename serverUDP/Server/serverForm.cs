using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing.Imaging;


namespace Server
{
   
    public partial class serverForm : Form
    {
        public byte my_hostid = 0;
        struct ClientInfo
        {
            public EndPoint endpoint;   
            public byte id;             
            public byte[] cntr;
        }
        gpad gp;
        
        List<ClientInfo> clientList;

        Socket serverSocket;
        byte[] receivedData = new byte[10];
        bool test_with_virtual_driver=false;
        //byte[] sendData = new byte[256];
        public serverForm()
        {
            clientList = new List<ClientInfo>();
            InitializeComponent();
        }

    private void Form1_Load(object sender,EventArgs e)
    {

            CheckForIllegalCrossThreadCalls = false;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1000);
            serverSocket.Bind(ipEndPoint);

            
            string[] ip = "174.74.91.23".Split('.'); //ESTE ENDPOINT É DO SERVER MSM
            IPAddress ipip = new IPAddress(new byte[] { byte.Parse(ip[0]), byte.Parse(ip[1]), byte.Parse(ip[2]), byte.Parse(ip[3]) });
            IPEndPoint ipeSender = new IPEndPoint(ipip, 1000);//porta 1000
            EndPoint epCentral_server = (EndPoint)ipeSender;

            if (test_with_virtual_driver)
                gp = new gpad();

            if ((my_hostid = Hand_shake_host(epCentral_server))!= 255)//(the central server must provide its (host avaliavle) id)
                serverSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref epCentral_server, new AsyncCallback(OnReceive), epCentral_server);
            else
                Close();
        }
    private void OnReceive(IAsyncResult ar)
    {
        IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 1000);
        EndPoint epCentral_server = (EndPoint)ipeSender;
        serverSocket.EndReceiveFrom (ar, ref epCentral_server);
        switch ((char)receivedData[0])
        {
            case 'c': // controle // ENVIA O ID JUNTO 
                gp.set_cntr(receivedData[2], (ushort)((receivedData[3] << 8) + receivedData[4]));

                    //for (int i=0;i<clientList.Count;i++)
                    //if (clientList[i].id == receivedData[2]){
                    //        clientList[i].cntr[0]= receivedData[3]; clientList[i].cntr[1] = receivedData[4];
                    //    if (test_with_virtual_driver)
                    //        gp.set_cntr(clientList[i].id, (ushort)((clientList[i].cntr[0] << 8) + clientList[i].cntr[1]));
                    //    else
                            textBox1.Text = ((receivedData[3] << 8) + receivedData[4]).ToString();
                    //    break;
                    //}
                break;
        }
        serverSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref epCentral_server, new AsyncCallback(OnReceive), epCentral_server);


    }
    public void Sending(EndPoint epCentral_server)
    {
            
        //Rectangle rect = new Rectangle(0, 0, 100, 100);
        //Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format16bppGrayScale);
        //Graphics g = Graphics.FromImage(bmp);
        //g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

        //MemoryStream ms = new MemoryStream();
        //bmp.Save(ms, ImageFormat.Jpeg);
        //byte[] bmpBytes = ms.GetBuffer();
        //bmp.Dispose();
        //ms.Close(); //g.Dispose();
        //int total = 0;
        //int size = bmpBytes.Length;
        //int sobra = size;
                
        //byte[] datasize = new byte[4];
        //datasize = BitConverter.GetBytes(size);
        //int enviado= serverSocket.SendTo(datasize,epCentral_server);// envia os tamanhos
        //while (total < size)
        //{
        //    enviado = serverSocket.SendTo(bmpBytes, total, sobra, SocketFlags.None,epCentral_server);
        //    total += enviado;
        //    sobra -= enviado;
        //}

    }
    private byte Hand_shake_host(EndPoint ep)
        {
            char[] a = {'h','_'};
            var byteData = Encoding.ASCII.GetBytes(a);
            for (int q = 1; q < 100; q++) serverSocket.SendTo(byteData, ep);
            byte aa = 1;
            while (aa < 10)
            {
                serverSocket.ReceiveFrom(byteData, ref ep);
                var az = new string(Encoding.UTF8.GetString(byteData).ToCharArray());
                if (az.Contains("OK")) { aa++; return (byte)az.ToCharArray()[2]; }
            }return (byte)255;
        }
    }
    
}