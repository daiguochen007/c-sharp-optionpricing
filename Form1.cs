using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace c_sharp_windows
{
    public partial class Form1 : Form
    {
        bool lan = true;
        public Form1()
        {
            InitializeComponent();
            Text ="期权计算器 - 戴国晨";           
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
//构造max函数
           static float max(float a, float b)
           {
               float max=a;
               if (max < b){max = b;}
               return max;
           }
//计算option price
        private void button2_Click(object sender, EventArgs e)
        {
            //检验是否填写完全
            foreach (Control c in this.Controls)
            {
                if (c is TextBox && c != textBox3 && c!= textBox8)
                {
                    if (string.IsNullOrEmpty((c as TextBox).Text))
                    {
                        if (lan == false)
                        { MessageBox.Show("You need to enter all the initial value."); }
                        else { MessageBox.Show("你还有初始值没输入"); }
                        return;//跳出事件
                    }
                }
            }
            if (this.radioButton2.Checked == false && this.radioButton1.Checked == false && this.checkBox1.Checked == false)
            {
                if (lan == false)
                { MessageBox.Show("Please pick up the type of option."); }
                else { MessageBox.Show("请选择期权类型"); }
                return;//跳出事件
            }
    //检查完毕开始读取参数
            float k = Convert.ToSingle(this.textBox1.Text);
            int n = Convert.ToInt32(this.textBox2.Text)+1;
            float szero = Convert.ToSingle(this.textBox6.Text);
            float u = Convert.ToSingle(this.textBox5.Text);
            float d = Convert.ToSingle(this.textBox4.Text);
            float r = Convert.ToSingle(this.textBox7.Text);
    //检验参数
          if ((1+r)>u||(1+r)<d)
            {
                if (lan == false)
                { MessageBox.Show("There is an arbitrage oppotunity."); }
                else { MessageBox.Show("存在无风险套利机会"); }
            }
    //参数入检验完了，开始计算
	        int i,j,a,b;
            float p,q,maxnum;
            float[,]s = new float[n,n];//c#居然可以定义动态数组
            float[,]v = new float[n,n];
            //s,v赋初值
            for(i = 0; i<n; i++)
             {for(j = 0; j<n; j++)
              {
              s[i,j] = 0;
              v[i,j] = 0;
              }}
	        s[0,0]=szero;

            //risk nertual probability（通用）
            p = (1 + r - d) / (u - d);
            q = 1 - p;

            //caculate S（通用）
            for (i = 1; i <= n - 1; i++)
            {
                s[0, i] = s[0, i - 1] * u;
                s[i, i] = s[i - 1, i - 1] * d;
                for (j = i + 1; j <= n - 1; j++)
                { s[i, j] = s[i, j - 1] * u; }
            }
//算普通期权
            if (this.radioButton2.Checked == false && this.radioButton1.Checked == false)
            { this.textBox3.Text = " "; }
            else { 
                 //caculate V of vanilla option
            for (j = 0; j <= n - 1; j++)
            {
                if (this.radioButton2.Checked == false)
                { v[j, n - 1] = max(s[j, n - 1] - k, 0); }//call
                else { v[j, n - 1] = max(k - s[j, n - 1], 0); }//put
            }
            for (i = n - 2; i >= 0; i--)
            {
                for (j = 0; j <= i; j++)
                {
                    v[j, i] = (v[j, i + 1] * p + q * v[j + 1, i + 1]) / (1 + r);
                }
            }
            this.textBox3.Text = Convert.ToString(v[0, 0]);            
            }
//算回望期权
            if (this.checkBox1.Checked == true)          
            {
                int[,] sign = new int[2, n];//lookback opt用
                for (j = 0; j <= n - 1; j++)//初始化sign 
                {
                    sign[0, j] = 0;
                    sign[1, j] = j;
                }
                int size = Convert.ToInt32(Math.Pow(2, n - 1));//动态数组和后面用
                float[,] rou = new float[size, n];
                float[,] l = new float[size, n];
                for (i = 0; i <= size - 1; i++)//初始化rou和l 
                {
                    for (j = 0; j <= n - 1; j++)
                    {
                        rou[i, j] = 0;
                        l[i, j] = 0;
                    }
                }
                //定义pick并初始化
                int[,] pick = new int[2, size];
                for (j = 0; j <= n - 1; j++)
                {
                    pick[0, j] = j;
                    pick[1, j] = 0;
                }
                for (i = 1; i <= size - 1; i++)
                {
                    if (Math.Log(i, 2) == Math.Floor(Math.Log(i, 2)))
                    {
                        pick[1, i] = n - 1 - Convert.ToInt32(Math.Log(i, 2));
                    }
                    else if (i % 2 == 1)
                    {
                        pick[1, i] = n - 1;
                    }
                    for (a = 2; a < size; a++)
                    {
                        if (i > Math.Pow(2, a) && i < Math.Pow(2, a + 1))
                        {
                            b = i - Convert.ToInt32(Math.Pow(2, a));
                            pick[1, i] = pick[1, b];
                        }
                    }
                }//初始化pick完毕

                //caculate Route of lookback option
                for (j = 0; j <= n - 1; j++)
                { rou[0, j] = s[0, j]; }

                for (i = 1; i <= size - 1; i++)
                {
                    for (j = 0; j <= n - 1; j++)
                    {

                        if (j == pick[1, i])//i和j满足一定关系挑出
                        {
                            sign[0, j] = sign[0, j] + 1;
                            for (a = j; a <= n - 1; a++)
                            { sign[0, a] = sign[0, j]; }
                        }
                        rou[i, j] = s[sign[0, j], sign[1, j]];
                    }
                }
                //caculate L of lookback option
                for (i = 0; i <= size - 1; i++)
                {
                    maxnum = 0;
                    for (j = 0; j <= n - 1; j++)
                    { maxnum = max(rou[i, j], maxnum); }
                    l[i, n - 1] = maxnum - rou[i, n - 1];
                }
                for (j = n - 2; j >= 0; j--)
                {
                    for (i = 0; i <= size / 2 - 1; i++)
                    {
                        l[i, j] = (p * l[2 * i, j + 1] + q * l[2 * i + 1, j + 1]) / (1 + r);
                    }
                }
                this.textBox8.Text = Convert.ToString(l[0, 0]);
            }
            else { this.textBox8.Text = " "; }

        }

//只允许输入数字
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar!=46)
                e.Handled = true;
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
                e.Handled = true;
        }
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
                e.Handled = true;
        }
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
                e.Handled = true;
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
                e.Handled = true;
        }
        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
                e.Handled = true;
        }

        private void 中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text = "期权计算器 - 戴国晨";
            this.label1.Text = "执行价K";
            this.label2.Text = "步数n";
            this.label3.Text = "现价S0";
            this.label4.Text = "上涨u";
            this.label5.Text = "下跌d";
            this.label6.Text = "普通期权价格";
            this.label9.Text = "回望期权价格";
            this.label7.Text = "利率r";
            this.radioButton1.Text = "看涨";
            this.radioButton2.Text = "看跌";
            this.checkBox1.Text = "回望";
            this.button1.Text = "退出";
            this.button2.Text = "计算";
            lan = true;
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text = "Option caculator - Guochen Dai";
            this.label1.Text = "Exercise price K";
            this.label2.Text = "Step n";
            this.label3.Text = "Present price S0";
            this.label4.Text = "Up u";
            this.label5.Text = "Down d";
            this.label6.Text = "Vanilla option price";
            this.label9.Text = "Lookback option price";
            this.label7.Text = "Interest rate r";
            this.radioButton1.Text = "call";
            this.radioButton2.Text = "put";
            this.checkBox1.Text = "lookback";
            this.button1.Text = "Exit";
            this.button2.Text = "Caculate";
            lan = false;
        }

    }
}
