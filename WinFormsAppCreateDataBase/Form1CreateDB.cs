using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibraryDB_Sql_code_first;
using Microsoft.EntityFrameworkCore;

//Задание №1
//Создайте серверное приложение с помощью, которого можно узнавать цены 
//на компьютерные комплектующие. Типичный пример работы:
//• клиентское приложение подключается к серверу;
//• клиентское приложение посылает запрос о цене конкретной запчасти 
//(например, цена на процессор);
//• сервер возвращает ответ;
//• клиент может послать новый запрос или отключиться. 
//Одновременно к серверу может быть подключено большое количество 
//клиентов. Используйте UDP сокеты для решения этой задачи.

namespace WinFormsAppCreateDataBase
{
    public partial class FormCreateDB : Form
    {
       
        /// <summary>
        /// строка подключения
        /// </summary>
       private string conectionString;

        public FormCreateDB()
        {
            InitializeComponent();

            conectionString = string.Empty;
            // set MarqueeAnimationSpeed
            progressBar1.MarqueeAnimationSpeed = 30;

            // set Visible false before you start long running task
            progressBar1.Visible = false;

          
        }
        /// <summary>
        /// Кнопка создания базы данных и загрузки ее в дата грид
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreateDB_Click(object sender, EventArgs e)
        {

            
           Task.Run(async ()=>
         
           {
               //делаем имитацию загрузки базы данных что бы пользователю было понятно что программа не зависла и выполняет работу
               //запускаем прогрес бар с помощью делегата
               this.progressBar1.Invoke(new Action(progressBar1.Show));
                try
               {
                   //строка подключения для упрощения работы и поиска. Как коментарий для себя
                   // "Server=(localdb)\\MSSQLLocalDB;Database=AdressForLabWork;Trusted_Connection=true";
                 
                   conectionString = textBox1.Text;
                   
                
                   DbContextOptionsBuilder<WarehouseOfSparePartsForComputers_Context> optionsBuilder = new DbContextOptionsBuilder<WarehouseOfSparePartsForComputers_Context>();
                   //Инфо для себя
                   //обязательно для работы этого метода доустановить пакет Microsoft.EntityFrameworkCore.SqlServer
                   optionsBuilder.UseSqlServer(conectionString);


                   var options = optionsBuilder.Options;
                   //создаем лист для создания базы данных
                   List<PartsPC> list = new List<PartsPC>() {
                       new PartsPC { CPU = "Процессор Пентиум",Frequency="1MHz",Cache_memory="L1", Price = 128 },
                       new PartsPC { CPU = "Процессор Пентиум",Frequency="2MHz",Cache_memory="L2", Price = 500 },
                       new PartsPC { CPU = "Процессор Athlon",Frequency="1MHz",Cache_memory="L3", Price = 52 },
                       new PartsPC { CPU = "Процессор Athlon",Frequency="5MHz",Cache_memory="L3", Price = 80 },
                       new PartsPC { CPU = "Процессор Test",Frequency="888MHz",Cache_memory="L999", Price = 1 },
                       


                   };

                   using (WarehouseOfSparePartsForComputers_Context context = new WarehouseOfSparePartsForComputers_Context(options))
                   {

                    
                       var table = context.Spare_parts_warehouse.ToList();
                       //если база пустая заполняем ее
                       if (table.Count == 0)
                       {
                           context.Spare_parts_warehouse.AddRange(list);
                       }
                       //если же не пустая то ищем повторяющиеся обьекты и в случае отсутсвия совпадения добаялем новые данные
                       else
                       {
                           //код добавления обьекта в таблицу если в будующем придет запрос на добавление 
                           //по условию этой задачи такое не требуется но все же на будующее предусматриваю доработку
                           foreach (var item in list)
                           {
                               var data = context.Spare_parts_warehouse.Where(x => x.Price == item.Price && x.CPU == item.CPU && x.Cache_memory == item.Cache_memory).SingleOrDefault();
                               if (data == null)
                               {
                                   context.Spare_parts_warehouse.Add(item);
                               }
                           }

                       }
                      
                      
                       context.SaveChanges();

                   }
             

                   await using (WarehouseOfSparePartsForComputers_Context context = new WarehouseOfSparePartsForComputers_Context(options))
                  {
                       context.Spare_parts_warehouse.Load<PartsPC>();
                       //тест создания базы данных
                       //Загружаем первый элемент для проверки создания БД

                       var parts = context.Spare_parts_warehouse.FirstOrDefault();
                       //перетягиваем с бд всю таблицу
                       var table = context.Spare_parts_warehouse.ToList();

                       //гурзим из бд в таблицу на форму
                       dataGridView1.Invoke(new MethodInvoker(() =>
                       {
                           dataGridView1.AutoGenerateColumns = true;

                           dataGridView1.DataSource = table;
                           //выравниваем таблицу по размеру текста из БД
                           dataGridView1.AutoSizeColumnsMode =
      DataGridViewAutoSizeColumnsMode.AllCells;
                       }));

                     

                       if (parts != null)
                       {
                           //останавливаем прогрес бар с помощью делегата
                           this.progressBar1.Invoke(new Action(progressBar1.Hide));
                           MessageBox.Show(" База данных создана","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                       }
                       else
                       {
                           //останавливаем прогрес бар с помощью делегата
                           this.progressBar1.Invoke(new Action(progressBar1.Hide));
                           MessageBox.Show("База данных запчасти не была успешно создана ");
                       }
                  }
               }

               catch (Exception ex)
               {
                   //останавливаем прогрес бар с помощью делегата
                   this.progressBar1.Invoke(new Action(progressBar1.Hide));
                   MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }


           });

        }
       
    }
}
    

