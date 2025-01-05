//Version 2.0
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace CompilerDefult
{
    internal class Program
    {
        public static List<string> MettadataException = new List<string>();
        public static void Main(string[] args)
        {
            if (args.Length == 0) { return; }
            try {
                Console.Title = "Компилятор V2.0";
                Console.WriteLine("Компилятор V2.0");
                foreach (string arg in args)
                {
                    Console.WriteLine("Библиотеки: для "+arg);
                    List<string> librarys = ArrayLibrary(File.ReadAllText(arg));
                    foreach (string lib in librarys)
                    {
                        Console.WriteLine("\t"+lib);
                    }
                    Console.WriteLine();
                    Compilers(File.ReadAllText(arg));
                    MettadataException.Clear();
                    Console.WriteLine("===============Компиляция заршена================");
                    Console.WriteLine();
                }
                Console.WriteLine("\nВсе операции компеляции завершены! Прочитайте логи");
                Console.ReadLine();
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
            
            
        }
        public static void Compilers(string content)
        {
            Console.WriteLine("Компиляция...");
            // Настройки компиляции
            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", "v4.0");
            
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.GenerateInMemory = true;
            compilerParams.GenerateExecutable = false;
            //вроде жалуется антивирус
            compilerParams.IncludeDebugInformation = false;
            List<string> librarys = ArrayLibrary(content);
            foreach (string lib in librarys)
            {
                if (!MettadataException.Contains(lib)) {
                    compilerParams.ReferencedAssemblies.Add(lib);
                }
            }
            MettadataException.Clear();
            // Компиляция
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, content);
            Console.WriteLine("Компиляция завершена");
            //вывод результата
            if (results.Errors.HasErrors == false)
            {
                Console.WriteLine("Всё найс");
            }
            //вывод ошибки
            else
            {
                Console.WriteLine("Ошибки: ");
                foreach (CompilerError err in results.Errors)
                {
                    if (err.ErrorNumber == "CS0006") {
                        if (!MettadataException.Contains("CS0006"))
                        {
                            MettadataException.Add("CS0006");
                        }
                        Console.WriteLine("\t"+err.ErrorText);
                        MettadataException.Add(err.ErrorText.Split('\"')[err.ErrorText.Split('\"').Length - 2]);
                    }
                    else
                    {
                        Console.WriteLine("\t" + err.Line);
                        Console.WriteLine("\t" + err.Column);
                        Console.WriteLine("\t" + err.ErrorNumber);
                        Console.WriteLine("\t" + err.ErrorText);
                    }
                }
                if (MettadataException.Contains("CS0006")) {
                    Console.WriteLine("Запуск повторной компиляции\n");
                    Compilers(content);
                }
            }
        }
        //Добавление библиотек
        private static List<string> ArrayLibrary(string content)
        {
            List<string> libList = new List<string>();
            
            string[] lines = content.Split('\n');
            foreach (string line in lines)
            {
                if (line.Contains("using") && line.Contains(";"))
                {
                    string temp = "";
                    string newline = line.Replace("using", "").Replace(";", "").Trim();
                    for (int i = 0; i < newline.Split('.').Length; i++)
                    {
                        temp += newline.Split('.')[i] + ".";
                        if (!libList.Contains(temp + "dll"))
                        {
                            libList.Add(temp + "dll");
                        }
                    }
                }
            }
            return libList;
        }
    }
}