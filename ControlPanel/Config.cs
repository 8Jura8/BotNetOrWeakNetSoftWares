using System;
using System.IO;
using System.Reflection;

namespace ControlPanel
{
    internal class Config
    {
        public static string ServerTitleComand { get; set; }
        public static string ServerTitleContext { get; set; }
        public static string SecretKey { get; set; }
        public static char   Spliter { get; set; } = '#';
        public static string AuthFile { get; } = "TokenData";
        public static string AuthOutputFile { get; } = "OutputTokenData";
        public static string KeyFile { get; } = "KeyData";
        public static string ComandFile { get; } = "ComandData";
        public static string MethodDefinitionFile { get; } = "StockMethodIsOnline";
        public static string DirectoryWebScripts { get; } = Path.Combine(Directory.GetCurrentDirectory() ,"WebScripts");

    }
}
