using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO.Ports;
using System.Management;

namespace tbnv
{
    public partial class Form1 : Form
    {
        String Data = "";
        public Form1()
        {
            InitializeComponent();
            this.Text = "UART COMMUNICATION";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] myport = SerialPort.GetPortNames();
            comboPORT.Items.AddRange(myport);// get port
            
        }

        private void Connect_Click(object sender, EventArgs e) // connect bt
        {
            if (comboPORT.Text == "")
            {
                MessageBox.Show("Please, Select your COM PORT!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBaudrate.Text == "")
            {
                MessageBox.Show("Please, Select your BaudRate!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                //serialPort1.PortName = comboPORT.Text;
                //serialPort1.BaudRate = Convert.ToInt32(comboBaudrate.Text); // convert to text
                
                if (serialPort1.IsOpen)
                {   
                    serialPort1.Close();
                    comboPORT.Enabled = true; 
                    comboBaudrate.Enabled = true;
                   
                    Connect.Text = "Connect";
                    progressBar1.Value = 0;
                    pictureBox6.Image = tbnv.Properties.Resources.OFF_1;
                    MessageBox.Show("Your Com Port is closed.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    serialPort1.PortName = comboPORT.Text;
                    serialPort1.BaudRate = int.Parse(comboBaudrate.Text); // Int.Parse(comboBaudrate.Text)
                    comboPORT.Enabled = false;
                    comboBaudrate.Enabled = false;
                    
                    serialPort1.Open();
                    progressBar1.Value = 100;
                    Connect.Text = "Disconnect";
                    MessageBox.Show("Your Com Port is connected and ready for use.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
               
            }
            catch (Exception)
            {
               
                MessageBox.Show("Your Com Port is not found. Please check your Cable or COM.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) // data nhan
        {
            //Data = serialPort1.ReadLine();
            this.Invoke(new EventHandler(ShowData));
            pictureBox6.Image = tbnv.Properties.Resources.ON_1;
        }

        private void ShowData(object sender, EventArgs e)
        {
            //textData.Text = Data;
            //serialPort1.ReadLine();
            string a = serialPort1.ReadExisting(); 
            Data += a;
            if (Data.Length > 62)
            {
                try// hàm bắt lỗi data json
                {
                   // Invoke(new MethodInvoker(() => listBox1.Items.Add("Data VDK: ")));
                    Invoke(new MethodInvoker(() => listBox1.Items.Add(Data)));

                    //textData.Text = Data;
                    var Datajson = JsonConvert.DeserializeObject<dynamic>(Data.Trim()); 
                                                                                        //Datajson.a
                    string nd = Datajson.ND; // lấy data json của từng nhãn
                    string da = Datajson.DA;
                    text_DA.Text = da + " % ";
                    text_ND.Text = nd + " ° C";
                    //text_C1.Text = Datajson.C1;

                    if (Datajson.TB1 == "0")
                    {
                        LED1.Text = "ON";
                        pictureBox1.Image = tbnv.Properties.Resources.off1;
                    }
                    else
                    {
                        LED1.Text = "OFF";
                        pictureBox1.Image = tbnv.Properties.Resources.on1;
                    }
                    if (Datajson.TB2 == "0")
                    {
                        LED2.Text = "ON";
                        pictureBox2.Image = tbnv.Properties.Resources.off1;
                    }
                    else
                    {
                        LED2.Text = "OFF";
                        pictureBox2.Image = tbnv.Properties.Resources.on1;
                    }
                    if (Datajson.IFAN == "0")
                    {
                        FAN.Text = "OFF";
                        WARNING.BackColor = Color.SpringGreen;
                    }
                    else
                    {
                       WARNING.BackColor = Color.Crimson;
                        FAN.Text = "ON";
                    }
                    // đây là show data ra
                    TEXTFAN.Text = Datajson.C1 +  " ° C";
                    Data = "";
                }
                catch (Exception)
                {

                }
            }

            //Data = serialPort1.ReadLine();

        }

        private void LED1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (LED1.Text == "ON")
                {
                    serialPort1.WriteLine("{\"TB1\":\"1\"}");
                    LED1.Text = "ON";
                    //LED1.BackColor = Color.G;
                }
                else
                {
                    serialPort1.WriteLine("{\"TB1\":\"0\"}");
                    LED1.Text = "OFF";
                }
            }
            else
            {
                MessageBox.Show("Please Connect your Com Port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LED2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (LED2.Text == "ON")
                {
                    serialPort1.WriteLine("{\"TB2\":\"1\"}");
                    LED2.Text = "ON";
                }
                else
                {
                    serialPort1.WriteLine("{\"TB2\":\"0\"}");
                    LED2.Text = "OFF";
                }
            }
            else
            {
                MessageBox.Show("Please Connect your Com Port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSend1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen && text_S1.Text != "")
            {
                serialPort1.WriteLine("{\"C1\":\"" + text_S1.Text.ToString().Trim() + "\"}");
                //TEXTFAN.Text = text_S1.Text + " ° C"; // hien thi thong so cai dat
                text_S1.Text = "";
            }
            else if (!serialPort1.IsOpen && text_S1.Text == "")
            {
                MessageBox.Show("Please, Enter your value and Connect your Com Port!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (text_S1.Text == "")
            {
                MessageBox.Show("Please, Enter your value!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                MessageBox.Show("Please Connect your Com Port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CLEAR_Click(object sender, EventArgs e)
        {
            listBox1.DataSource = null;
            listBox1.Items.Clear();
        }

        private void splitContainer4_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // hop thoai hỏi đóng 
        {
            DialogResult answer = MessageBox.Show("Do you want to exit the program ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
            }
        }
    }
}
