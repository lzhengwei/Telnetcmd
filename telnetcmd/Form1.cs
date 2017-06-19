using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace telnetcmd
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static NetworkStream stream;
        public static TcpClient tcpclient;
        public static string ip;
        public static int port;
        Socket SckSPort; // 先行宣告Socket
        StreamReader sr ;
        StreamWriter sw;
        RichTextBox rb = new RichTextBox();
        Thread SckSReceiveTd;
        string RmIp;
        string value = "", cmdin = "";
        public delegate void mydalegate();
        public mydalegate md;
        string s;
        bool streamout = false;
        int SPort = 6101;


        private void button1_Click(object sender, EventArgs e)
        {
            telnetstart();
        }
      


        public void telnetstart()
        {
            Console.WriteLine("目標IP:");
            // ip = textBox1.Text;
            if(InputBox("IP輸入", "輸入目標主機IP address : ", ref value)
                ==DialogResult.OK)
            ip = value;

            Console.WriteLine("目標Port:");
            // port = int.Parse(textBox2.Text);
            port = 10000;
            label1.Text = "連線成功!!\n目標的IP : " + ip 
                + "\n\n目標的Port : " + port.ToString();

            tcpclient = new TcpClient(ip, port);  // 連接服務器
            stream = tcpclient.GetStream();   // 獲取網絡數據流對象
               sw = new StreamWriter(stream); //將輸入資料傳到sever
               sr = new StreamReader(stream, Encoding
                   .GetEncoding(950));//讀取sever端回傳的資訊
                                      //且編碼為big-5
            
           SckSReceiveTd = new Thread(Runtelnet);//將傳資料及讀資料
                                                 //的function改為用
                                                 //多執行緒執行
            SckSReceiveTd.Start();
     
        }

       public void Runtelnet()
        {

                   while (true)
                {
                    stream.ReadTimeout = 10;
                   
                    try
                    {
                        while (!sr.EndOfStream)   //判斷是否還有資料未接收
                        {

                           string c = sr.ReadLine();
                            s = s+"\n"+c;        //讀取資料存在S裡                     
                            Console.WriteLine(c);
                        }
                    }
                    catch(Exception e)
                    {
                     
                    }
                    streamout = true;

                    if (!String.IsNullOrEmpty(cmdin))
                    {
                        sw.Write("{0}\r\n", cmdin);  //傳入指令，指令存在cmdin裡
                        cmdin = "";
                    }
                    sw.Flush();   //傳送端資料更新
     
                }
            


        }
       private void button3_Click(object sender, EventArgs e)
       {
            InputBox("指令視窗", "輸入指令 : ", ref value);
            form.Close();
          /* if (inputresult == DialogResult.OK)
           {
               cmdin = value;
           }*/
          
       }    

    

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (streamout)  //timer會固定來判斷是否有資料
            {
                
                richTextBox1.Text = richTextBox1.Text + s;
                streamout = false;//有資料時輸出至richtextBox
                s = "";
            }
        }
        public Form form;
        public  DialogResult InputBox(string title, string promptText, ref string value)
        {
            
             form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            ListBox listboxm = new ListBox();
            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            
            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = ""; //將連線斷線初始化
            label1.Text = "尚未連接";
            SckSReceiveTd.Abort();
            stream.Close();
            tcpclient.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
          
            cmdin = "netstat -an";//獲取PORT資訊
        }

        private void button5_Click(object sender, EventArgs e)
        {
        
            cmdin = "ipconfig";//獲取IP
        }
        private void button2_Click(object sender, EventArgs e)
        {
            cmdin = "shutdown -s";//將sever關機
        }

        private void button7_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";//清除畫面
        }



     

   







    }
}
