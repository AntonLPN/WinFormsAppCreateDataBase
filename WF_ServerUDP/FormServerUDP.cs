using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using ClassLibraryDB_Sql_code_first;
using System.Text.Json;
using System.Threading;
using System.Text.Json.Serialization;

namespace WF_ServerUDP
{
    public partial class FormServerUDP : Form
    {
        /// <summary>
        /// Задача получения запроса
        /// </summary>
        Task receiver;
        /// <summary>
        /// Порт подключения
        /// </summary>
        int port;
        int portSend;

        public FormServerUDP()
        {
            InitializeComponent();

            receiver = null;
            port = 11000;
            portSend = 11001;
        }

        private  void buttonStartServerUDP_Click(object sender, EventArgs e)
        {
            try
            {
                if (receiver!=null)
                {
                    return;
                }
                else
                {
                    textBox1.Text = "Сервер запущен, время " + DateTime.Now.ToShortTimeString() + Environment.NewLine;
                    receiver = Task.Run(async() =>
                    {

                        UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Loopback,port));
                        IPEndPoint iPEndPoint = null;

                        while (true)
                        {
                            byte[] buff = listener.Receive(ref iPEndPoint);
                            StringBuilder builder = new StringBuilder();

                            // builder.AppendLine($"- Получено сообщение от {iPEndPoint}");
                            builder.AppendLine(Encoding.Default.GetString(buff));
                            textBox1.BeginInvoke(new Action<string>(AddText), builder.ToString());

                           
                            string selectDBPrice =builder.ToString();
                            //удаляем \r\n
                            selectDBPrice= selectDBPrice.Replace(System.Environment.NewLine, string.Empty);
                            DbContextOptionsBuilder<WarehouseOfSparePartsForComputers_Context> optionsBuilder = new DbContextOptionsBuilder<WarehouseOfSparePartsForComputers_Context>();
                            //Инфо для себя
                            //обязательно для работы этого метода доустановить пакет Microsoft.EntityFrameworkCore.SqlServer
                            //Краткий коментарий к коду по дебагу
                            //Шел 5 час борьбы с подключением к базе данных в итоге понял
                            //что подключаюсь не к базе а к таблице
                            //ДА ЧТО ВЫ ЗНАЕТЕ О БОЛИ)))))))
                            //МАГИЯ НЕ ТРОГАТЬ
                            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WarehouseOfSparePartsForComputers_Context;Trusted_Connection=True;");


                            var options = optionsBuilder.Options;

                            await using (WarehouseOfSparePartsForComputers_Context context = new WarehouseOfSparePartsForComputers_Context(options))
                            {
                                context.Spare_parts_warehouse.Load<PartsPC>();
                                var selectedCPU = context.Spare_parts_warehouse.FromSqlRaw($"SELECT * FROM Spare_parts_warehouse WHERE Price = {selectDBPrice}");
                                //var selectedCPU =  context.Spare_parts_warehouse.FromSqlRaw(selectDBPrice).ToList();
                              
                                //  var selectedCPU = context.Spare_parts_warehouse.FromSqlRaw("SELECT * FROM Spare_parts_warehouse WHERE Price = 1");
                              
                                if (selectedCPU!=null)
                                {
                                    textBox1.BeginInvoke(new Action<string>(AddText), "Отправка данных клиенту");

                               
                                    string json = JsonSerializer.Serialize(selectedCPU);
                                    // сериализуем и отправлем 
                                    byte[] serialalData = Encoding.Unicode.GetBytes(json);
                                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Loopback, portSend);
                                    Thread.Sleep(8000);

                                    listener.Send(serialalData, serialalData.Length, remoteEP);
                                }
                                else
                                {
                                    textBox1.BeginInvoke(new Action<string>(AddText),"Товар с такой ценой не найден невозможно выполнить отправку данных");
                                }


                               
                                
                                listener.Close();
                                return;

                            }

                           
                        }                    
                    
                    });
                }

            }
            catch (ArgumentOutOfRangeException )
            {
                MessageBox.Show("Некорректный номер порта", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SocketException )
            {
                MessageBox.Show("Порт уже используется", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// метод добавления текста в текст бокс для использования в делегате
        /// </summary>
        private void AddText(string str)
        {
            StringBuilder builder = new StringBuilder(textBox1.Text);
            builder.AppendLine(str);
            textBox1.Text = builder.ToString();
        }

      
    }
}
