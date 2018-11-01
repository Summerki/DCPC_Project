using DirectConnectionPredictControl.CommenTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// Antiskid_Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Antiskid_Setting : Window // 防滑分析用户设置值界面
    {
        #region 与colse有关的
        public event closeWindowHandler CloseWindowEvent;
        private void Antiskid_Setting_Window_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "Antiskid_Setting_Window");
        }
        #endregion

        public Antiskid_Setting()
        {
            InitializeComponent();

        }



        #region window event
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ChartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            this.Width = x1 * 2 / 3;
            this.Height = y1 * 4 / 5;
        }

        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximunBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        /// <summary>
        /// 设置某一位的值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index">要设置的位， 值从低到高为 1-8</param>
        /// <param name="flag">要设置的值 true / false</param>
        /// <returns></returns>
        public byte set_bit(byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }

        public byte[] Byte_0x91 = new byte[8];
        public byte[] Byte_0x92 = new byte[8];
        public byte[] Byte_0x93 = new byte[8];
        public byte[] Byte_0x94 = new byte[8];

        /// <summary>
        /// 因为用户会输入小数，这个函数是为了能够发送出去，先将小数乘以10再发送
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Return_Ten_Plus(string text)
        {
            double temp = double.Parse(text);
            temp = temp * 10;
            string temp_1 = temp.ToString();
            return temp_1;
        }

        

        /// <summary>
        /// 将用户输入的值保存至Byte[]数组中
        /// </summary>
        private void SaveValue()
        {
            Byte_0x91[1] = Convert.ToByte(Return_Ten_Plus(TextBox_1.Text));
            Byte_0x91[2] = Convert.ToByte(Return_Ten_Plus(TextBox_2.Text));
            Byte_0x91[3] = Convert.ToByte(Return_Ten_Plus(TextBox_3.Text));
            Byte_0x91[4] = Convert.ToByte(Return_Ten_Plus(TextBox_4.Text));
            Byte_0x91[5] = Convert.ToByte(Return_Ten_Plus(TextBox_5.Text));
            Byte_0x91[6] = Convert.ToByte(Return_Ten_Plus(TextBox_6.Text));
            Byte_0x91[7] = Convert.ToByte(Return_Ten_Plus(TextBox_7.Text));

            Byte_0x92[0] = Convert.ToByte(Return_Ten_Plus(TextBox_8.Text));
            Byte_0x92[1] = Convert.ToByte(Return_Ten_Plus(TextBox_9.Text));
            Byte_0x92[2] = Convert.ToByte(Return_Ten_Plus(TextBox_10.Text));
            Byte_0x92[3] = Convert.ToByte(Return_Ten_Plus(TextBox_11.Text));
            Byte_0x92[4] = Convert.ToByte(Return_Ten_Plus(TextBox_12.Text));
            Byte_0x92[5] = Convert.ToByte(Return_Ten_Plus(TextBox_13.Text));
            Byte_0x92[6] = Convert.ToByte(Return_Ten_Plus(TextBox_14.Text));
            Byte_0x92[7] = Convert.ToByte(Return_Ten_Plus(TextBox_15.Text));

            // 减速度这里还有负值的情况，加个判断条件吧
            //Byte_0x93[0] = Convert.ToByte(Return_Ten_Plus(TextBox_16.Text));
            //Byte_0x93[1] = Convert.ToByte(Return_Ten_Plus(TextBox_17.Text));
            //Byte_0x93[2] = Convert.ToByte(Return_Ten_Plus(TextBox_18.Text));
            double textBox_16 = double.Parse(TextBox_16.Text);
            if (textBox_16 < 0) // 小于0的情况
            {
                double textBox_16_1 = -textBox_16;
                Byte_0x93[0] = Convert.ToByte(textBox_16_1);
                Byte_0x93[0] = set_bit(Byte_0x93[0], 8, true);
            }
            else { Byte_0x93[0] = Convert.ToByte(Return_Ten_Plus(TextBox_16.Text)); } // 这是大于等于0的情况

            double textBox_17 = double.Parse(TextBox_17.Text);
            if (textBox_17 < 0) // 小于0的情况
            {
                double textBox_17_1 = -textBox_17;
                Byte_0x93[1] = Convert.ToByte(textBox_17_1);
                Byte_0x93[1] = set_bit(Byte_0x93[1], 8, true);
            }
            else { Byte_0x93[1] = Convert.ToByte(Return_Ten_Plus(TextBox_17.Text)); } // 这是大于等于0的情况

            double textBox_18 = double.Parse(TextBox_18.Text);
            if (textBox_18 < 0) // 小于0的情况
            {
                double textBox_18_1 = -textBox_18;
                Byte_0x93[2] = Convert.ToByte(textBox_18_1);
                Byte_0x93[2] = set_bit(Byte_0x93[2], 8, true);
            }
            else { Byte_0x93[2] = Convert.ToByte(Return_Ten_Plus(TextBox_18.Text)); } // 这是大于等于0的情况

            Byte_0x93[3] = Convert.ToByte(TextBox_19.Text);
            Byte_0x93[4] = Convert.ToByte(TextBox_20.Text);
            Byte_0x93[5] = Convert.ToByte(TextBox_21.Text);
            Byte_0x93[6] = Convert.ToByte(TextBox_22.Text);
            Byte_0x93[7] = Convert.ToByte(TextBox_23.Text);

            Byte_0x94[0] = Convert.ToByte(TextBox_24.Text);
            Byte_0x94[1] = Convert.ToByte(TextBox_25.Text);
            Byte_0x94[2] = Convert.ToByte(TextBox_26.Text);
            Byte_0x94[3] = Convert.ToByte(TextBox_27.Text);
        }

        private bool bSend = false;// 2018-10-8:新增标志位，判断是否发送成功
        private CanHelper canHelper;
        /// <summary>
        /// 防滑分析设置界面用户输入完成之后点击“设置完成”按钮产生的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                canHelper = new CanHelper();
                //set_bit(Byte_0x91[0], 1, true);
                Byte_0x91[0] = 0x01;
                SaveValue();
                for (int i = 0; i < 10; i++)
                {
                    canHelper.Send_0x91(Byte_0x91);
                    canHelper.Send_0x92(Byte_0x92);
                    canHelper.Send_0x93(Byte_0x93);
                    canHelper.Send_0x94(Byte_0x94);
                }
                bSend = true;
            }
            catch (Exception)
            {
                MessageBox.Show("数据发送失败！");
                bSend = false;
            }
            if (bSend == true)
            {
                MessageBox.Show("数据发送成功！");
            }
        }

        #region TextChanged事件合集
        private void TextBox_1_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_2_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_3_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_4_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_5_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_6_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_7_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_8_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_9_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_10_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_11_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_12_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_13_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_14_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_15_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_16_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_17_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_18_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_19_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_20_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_21_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_22_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_23_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_24_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_25_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_26_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void TextBox_27_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    if (textBox.Text == "-")// 这是让textbox可以输入符号的情况
                    {
                        return;
                    }
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }


        #endregion

        
    }
}
