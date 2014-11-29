using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Chat_App
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint eplocal, epremote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //set up Socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //get user IP
            textLocalIP.Text = GetLocalIP();
            textRemoteIP.Text = GetLocalIP();
        }

            private string GetLocalIP()
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());

                return "127.0.0.1";

            }

            private void buttonConnect_Click(object sender, EventArgs e)
            {
                //biinding socket
                eplocal = new IPEndPoint(IPAddress.Parse(textLocalIP.Text), Convert.ToInt32(textLocalPort.Text));
                sck.Bind(eplocal);
                //connect to remote IP
                epremote = new IPEndPoint(IPAddress.Parse(textRemoteIP.Text), Convert.ToInt32(textRemotePort.Text));
                sck.Connect(epremote);
                //Listen the specific Host
                buffer = new byte[5000];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epremote, new AsyncCallback(MessageCallBack), buffer);
            }
            private void MessageCallBack(IAsyncResult aResult)
            {
                try
                {
                    byte[] receivedData = new byte[5000];
                    receivedData = (byte[])aResult.AsyncState;
                    //Converting byte[] to string
                    ASCIIEncoding aEncoding = new ASCIIEncoding();
                    string receivedMessage = aEncoding.GetString(receivedData);
                    //Adding this message to ListBox
                    listMessage.Items.Add("Friend:" + receivedMessage);
                    sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epremote, new AsyncCallback(MessageCallBack), buffer);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());

                }
            }

            private void buttonSend_Click(object sender, EventArgs e)
            {
                //Convert String Message to byte[]
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                byte[] sendingMessage = new byte[5000];
                sendingMessage = aEncoding.GetBytes(textMessage.Text);
                //Sending the encoded message
                sck.Send(sendingMessage);
                //adding to listbox
                listMessage.Items.Add("Me:" + textMessage.Text);
                textMessage.Text = "";


            }
            

        }

        
    }
