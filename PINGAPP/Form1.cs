using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Sockets;

namespace PINGAPP
{
    public partial class Form1 : Form
    {
        protected Ping ping = null;
        protected PingReply reply = null;
        protected PingOptions options = null;
        protected IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
        protected NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        private uint GetFirstOctet(IPAddress ipadr)
        {
            byte[] bytesIP = ipadr.GetAddressBytes();
            return (uint)bytesIP[0];
        }
        private string GetSubnetMask(IPAddress ipadr)
        {
            uint firstOctet = GetFirstOctet(ipadr);
                 if (firstOctet >= 0 && firstOctet <= 127)
                          return "255.0.0.0";
                 else if (firstOctet >= 128 && firstOctet <= 191)
                        return "255.255.0.0";
                   else if (firstOctet >= 192 && firstOctet <= 223)
                       return "255.255.255.0";
                  else return "0.0.0.0";
        }

       
        public Form1()
        {

            InitializeComponent();
            label1.Font = new Font("Arial", 10);
            label2.Font = new Font("Arial", 10);
            richTextBox1.Font = new Font("Arial", 13);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();

            toolTip1.ShowAlways = true;


            toolTip1.SetToolTip(this.button1, "Clear results");
            toolTip1.SetToolTip(this.button2, "Check host accessibility");
            toolTip1.SetToolTip(this.textBox1, "Enter IP adress here");
            toolTip1.SetToolTip(this.textBox2, "Send ping request every x seconds");
            toolTip1.SetToolTip(this.button3, "Interfaces on network");
            toolTip1.SetToolTip(this.button4, "Get subnet mask of given IPadress");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            richTextBox1.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IPAddress IP;
            bool flag = IPAddress.TryParse(textBox1.Text, out IP);
            button2.Enabled = textBox1.TextLength > 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                ping = new Ping();
                options = new PingOptions();
                if (textBox2.Text.Equals(""))
                {
                    reply = ping.Send(textBox1.Text);
                }
                else
                {
                    reply = ping.Send(textBox1.Text, Int32.Parse(textBox2.Text));
                }
                long rtt = reply.RoundtripTime * 1000;
                if (reply.Status == IPStatus.Success)
                {
                    richTextBox1.Visible = true;
                    richTextBox1.Text += "Accessible." + '\n';

                    richTextBox1.Text += "Round trip time: ";
                  
                    richTextBox1.Text +=  reply.RoundtripTime;
                    richTextBox1.Text += '\n';
                
                    richTextBox1.Text += "TTL:" + options.Ttl;
                    richTextBox1.Text += '\n';
                }
                else
                {
                   
                    richTextBox1.Text += "Not accessible." + '\n';
                }
            }
            catch (PingException ex)
            {
                //PingException ex = new PingException("Greška");
                richTextBox1.Text += ex.Message;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            richTextBox1.Clear();
            richTextBox1.Text += "Interface information " + '\n';

            richTextBox1.Text += "Host name: ";
            richTextBox1.Text += computerProperties.HostName + '\n';

            if (nics == null || nics.Length < 1)
            {
                richTextBox1.Clear();

                richTextBox1.Text += "  No network interfaces found.";
                return;
            }
            richTextBox1.Text += "  Number of interfaces:";
            richTextBox1.Text += nics.Length.ToString() + '\n';
            
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                richTextBox1.Text += '\n';
                richTextBox1.Text += adapter.Description.ToString() + '\n';

                richTextBox1.Text += "  Interface type:";
                richTextBox1.Text += adapter.NetworkInterfaceType;
                richTextBox1.Text += '\n';
                richTextBox1.Text += "  Physical Address:";
                richTextBox1.Text += adapter.GetPhysicalAddress().ToString() + '\n';
                richTextBox1.Text += "  Operational status:";
                richTextBox1.Text += adapter.OperationalStatus + '\n';
                string versions = "";

                
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                richTextBox1.Text += "  IP version:";
                richTextBox1.Text += versions + '\n';

               
               
               
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (textBox1.Text!="") { 
            IPAddress[] addresses = Dns.GetHostAddresses(textBox1.Text);
            foreach (var ip in addresses)
            {
                richTextBox1.Text += "IP adress:" + ip + "Subnet mask:" + GetSubnetMask(ip);
            }
            
   }
            else
 richTextBox1.Text += "Please enter a valid IP adress to get subnet mask";     
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

