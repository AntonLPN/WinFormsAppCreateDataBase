using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using ClassLibraryDB_Sql_code_first;
using System.Text.Json.Serialization;

namespace WF_ClientUDP
{
    public partial class FormClientUDP : Form
    {
        UdpClient senderMessage;
        UdpClient catchClient;

        public FormClientUDP()
        {
            InitializeComponent();

          
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
          await  Task.Run(() =>
            {

                int port = 11000;
                // IPAddress remoteAddress=null;

                try
                {
                    senderMessage = new UdpClient(); // создаем UdpClient для отправки сообщений
                    if (textBox1.Text != " ")
                    {
                        //проверяем точно ли у нас циферное значение (АНТИБАГ)
                        double valuePrice = double.Parse(textBox1.Text);
                        //вообще не очень нужное телодвижение но лучше пользователю не доверять всякое может наклацать
                        string priceStr = valuePrice.ToString(); // сообщение для отправки
                                                                 //теперь делаем строку для отправки на сервер 
                        string message = priceStr;



                        byte[] data = Encoding.Default.GetBytes(message);
                        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Loopback, port);
                        senderMessage.Send(data, data.Length, remoteEP); // отправка
                        MessageBox.Show("Запрос отправлен", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        int portSend = 11001;
                        IPEndPoint remoteEPCatch = new IPEndPoint(IPAddress.Loopback, portSend);
                        // получаем ответ
                        data = new byte[1024]; // буфер для ответа



                        while (true)
                        {

                            catchClient = new UdpClient();
                            catchClient.Client.Bind(new IPEndPoint(IPAddress.Loopback, portSend));
                            byte[] buff = catchClient.Receive(ref remoteEPCatch);
                            StringBuilder builderReciver = new StringBuilder();
                            builderReciver.AppendLine(Encoding.Unicode.GetString(buff));

                            try
                            {

                                //десиариализация колекции 


                                string test = builderReciver.ToString();
                                var restoreProduct = JsonSerializer.Deserialize<List<PartsPC>>(test);

                                //гурзим из бд в таблицу на форму
                                dataGridView1.Invoke(new MethodInvoker(() =>
                                {
                                    dataGridView1.AutoGenerateColumns = true;

                                    dataGridView1.DataSource = restoreProduct;
                                    //выравниваем таблицу по размеру текста из БД
                                    dataGridView1.AutoSizeColumnsMode =
               DataGridViewAutoSizeColumnsMode.AllCells;
                                }));

                                catchClient.Close();
                                break;
                            }
                            catch (Exception ex)
                            {

                                MessageBox.Show(ex.Message);
                            }


                        }



                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }





            });
          

        }




        /// <summary>
        /// Запрет на ввод букв с разрешением запятой для получения значения типа double
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number!=8 && number!=44)//цифры и клавиша BackSpace и запятая
            {
                e.Handled = true;
            }
        }

        private void buttonDisconnectUDP_Click(object sender, EventArgs e)
        {
            catchClient.Close();
            senderMessage.Close();
            MessageBox.Show("Все соединения с сервером закрыты");
        }
    }
}
