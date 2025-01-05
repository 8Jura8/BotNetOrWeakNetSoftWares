using ControlPanel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlPanel
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> webScript = new Dictionary<string, string>();
        Dictionary<string, string> ContentOutputPages = new Dictionary<string, string>();
        List<string> Comand = new List<string>();
        TelegraphServer TelegraphServer;
        TelegraphServer TelegraphServerOutput;
        string YserName;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxYserName.Items.AddRange(new string[] { "WebName", "All" });
            comboBoxYserName.SelectedIndex = 1;
            checkedListBoxFilesInContext.CheckOnClick = true;
            GetFiles();
            if (File.Exists(Config.AuthFile))
            {
                textBoxRegistrationAccessToken.Text = File.ReadAllText(Config.AuthFile);
            }
            if (File.Exists(Config.KeyFile))
            {
                textBoxKeyEncode.Text = File.ReadAllText(Config.KeyFile);
            }
            if (string.IsNullOrEmpty(textBoxKeyEncode.Text))
            {
                textBoxKeyEncode.Text = "SwP9sph23b36cM";
            }
            if (File.Exists(Config.AuthOutputFile))
            {
                textBoxAccessTokenOutput.Text = File.ReadAllText(Config.AuthOutputFile);
            }
            if (File.Exists(Config.ComandFile))
            {
                comboBoxComand.Items.AddRange(File.ReadAllText(Config.ComandFile).Split('@'));
                comboBoxComand.Items.Remove(string.Empty);
                comboBoxComand.Items.Remove(null);
            }
        }
        private void GetComand()
        {
            if (File.Exists(Config.ComandFile))
            {
                comboBoxComand.Items.Clear();
                comboBoxComand.Items.AddRange(File.ReadAllText(Config.ComandFile).Split('@'));
                comboBoxComand.Items.Remove(string.Empty);
                comboBoxComand.Items.Remove(null);
            }

            foreach (string key in Comand)
            {
                if (!comboBoxComand.Items.Contains(key))
                {
                    comboBoxComand.Items.Add(key);
                }
            }
            string content = "";
            foreach (string item in comboBoxComand.Items)
            {
                if (!webScript.ContainsKey(item) && !string.IsNullOrEmpty(item))
                {
                    content += item + "@";
                }
            }
            File.WriteAllText(Config.ComandFile, content);
            Comand.Clear();
        }
        private void DelitComand(string comandName)
        {
            string content = "";
            List<string> items = new List<string>();
            foreach (string item in comboBoxComand.Items)
            {
                items.Add(item);
            }
            comboBoxComand.Items.Clear();
            foreach (string item in items)
            {
                if (item != comandName)
                {
                    comboBoxComand.Items.Add(item);
                    content += item + "@";
                }
            }
            comboBoxComand.Text = "";
            if (comboBoxComand.Items.Count == 0)
            {
                content = "";
            }
            else
            {
                content = content.Remove(content.Length - 1);
            }
            File.WriteAllText(Config.ComandFile, content);
        }
        private void GetFiles()
        {
            checkedListBoxFilesInContext.Items.Clear();
            comboBoxWebScript.Items.Clear();
            if (Directory.Exists(Config.DirectoryWebScripts))
            {
                foreach (string script in Directory.GetFiles(Config.DirectoryWebScripts))
                {
                    comboBoxWebScript.Items.Add(script.Split('\\')[script.Split('\\').Length - 1]);
                    checkedListBoxFilesInContext.Items.Add(script.Split('\\')[script.Split('\\').Length - 1]);
                }
            }
            else
            {
                Directory.CreateDirectory(Config.DirectoryWebScripts);
            }
        }
        private void buttonRegistrationToken_Click(object sender, EventArgs e)
        {
            string name = textBoxNewName.Text;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Ошибка при регистрации");
            }
            else
            {
                TelegraphServer = new TelegraphServer();
                TelegraphServer.CreateAccount(name);
                if (string.IsNullOrEmpty(TelegraphServer.Access_Token))
                {
                    MessageBox.Show("Ошибка: акаунт не создан");
                }
                else
                {
                    textBoxRegistrationAccessToken.Text = TelegraphServer.Access_Token;
                    MessageBox.Show("Акаунт создан!");
                }
            }
        }

        private void GetPages()
        {
            List<string> pages = TelegraphServer.GetPageList();

            listBoxSelectedPageToComand.Items.Clear();
            listBoxSelectedPageWebScript.Items.Clear();

            foreach (string page in pages)
            {
                listBoxSelectedPageWebScript.Items.Add(page);
                listBoxSelectedPageToComand.Items.Add(page);
            }
        }
        private void buttonActivationToken_Click(object sender, EventArgs e)
        {
            string registrationAccessToken = textBoxRegistrationAccessToken.Text;
            if (string.IsNullOrEmpty(registrationAccessToken))
            {
                MessageBox.Show("Ошибка при активации(токен)");
            }
            else
            {
                TelegraphServer = new TelegraphServer(registrationAccessToken);
                buttonChangeToken.Enabled = true;
                buttonActivationToken.Enabled = false;
                buttonSendContext.Enabled = true;
                buttonRefrrehPage.Enabled = true;
                buttonCreatePage.Enabled = true;
                buttonSendComand.Enabled = true;


                GetPages();
            }
        }
        private void buttonChangeToken_Click(object sender, EventArgs e)
        {
            buttonChangeToken.Enabled = false;
            buttonActivationToken.Enabled = true;

            listBoxSelectedPageToComand.Items.Clear();
            listBoxSelectedPageWebScript.Items.Clear();

            buttonRefrrehPage.Enabled = false;
            buttonCreatePage.Enabled = false;
            buttonSendComand.Enabled = false;
            buttonSendContext.Enabled = false;

            textBoxSelectedPageToComand.Text = string.Empty;
            textBoxSelectedPageWebScript.Text = string.Empty;

            labelContexTitle.Text = string.Empty;
            labelTitleComand.Text = string.Empty;
            labelCMD.Text = string.Empty;
        }
        private void buttonSaveToken_Click(object sender, EventArgs e)
        {
            File.WriteAllText(Config.AuthFile, textBoxRegistrationAccessToken.Text);
            File.WriteAllText(Config.KeyFile, textBoxKeyEncode.Text);
        }

        private void buttonCreatePage_Click(object sender, EventArgs e)
        {
            string namePage = textBoxCreatePage.Text;
            if (string.IsNullOrEmpty(namePage))
            {
                MessageBox.Show("ошибка создания");
            }
            else
            {
                TelegraphServer.CreatePage(namePage);
                GetPages();
            }
        }

        private void listBoxSelectedPageToComand_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxSelectedPageToComand.Text = listBoxSelectedPageToComand.Text;
            Config.ServerTitleComand = listBoxSelectedPageToComand.Text;
            labelTitleComand.Text = Config.ServerTitleComand;
            labelCMD.Text = EncryptionHelper.Decrypt(Web.SendGet(Config.ServerTitleComand), Config.SecretKey);
        }

        private void listBoxSelectedPageWebScript_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxSelectedPageWebScript.Text = listBoxSelectedPageWebScript.Text;
            labelContexTitle.Text = textBoxSelectedPageWebScript.Text;
            Config.ServerTitleContext = textBoxSelectedPageWebScript.Text;

            ;
        }

        private void buttonSendComand_Click(object sender, EventArgs e)
        {
            string cmd = textBoxWebName.Text + Config.Spliter + comboBoxComand.Text + Config.Spliter + textBoxSendComand.Text + Config.Spliter + new Random().Next(10000).ToString();
            if (string.IsNullOrEmpty(comboBoxComand.Text))
            {
                return;
            }
            if (TelegraphServer.EditPage(Config.ServerTitleComand, Config.ServerTitleComand, EncryptionHelper.Encrypt(cmd, Config.SecretKey)))
            {
                labelCMD.Text = cmd;
            }
            else
            {
                MessageBox.Show("Ошибка отправки \nКоманда:" + cmd);
            }
        }

        private void comboBoxWebScript_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pathFile = Path.Combine(Config.DirectoryWebScripts, comboBoxWebScript.Text);
            if (File.Exists(pathFile))
            {
                richTextBoxScriptCode.Text = File.ReadAllText(pathFile);
            }
            else
            {
                MessageBox.Show("Фаил не существует");
                GetFiles();
            }
        }

        private void buttoSaveCode_Click(object sender, EventArgs e)
        {
            try
            {
                string pathFile = Path.Combine(Config.DirectoryWebScripts, comboBoxWebScript.Text);
                File.WriteAllText(pathFile, richTextBoxScriptCode.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка Сохранения файла!");
            }
            GetFiles();

        }

        private void buttonSaveNewFile_Click(object sender, EventArgs e)
        {
            string pathFile = Path.Combine(Config.DirectoryWebScripts, textBoxNewFile.Text);
            if (!string.IsNullOrEmpty(textBoxNewFile.Text))
            {
                if (File.Exists(pathFile))
                {
                    MessageBox.Show("Такой файл уже существует!");
                }
                else
                {
                    FileStream file = File.Create(pathFile);
                    file.Close();
                    file.Dispose();
                    MessageBox.Show("фаил создан!");
                }
            }
            GetFiles();
        }

        private void buttonDelitFile_Click(object sender, EventArgs e)
        {
            try
            {
                string pathFile = Path.Combine(Config.DirectoryWebScripts, comboBoxWebScript.Text);
                if (!string.IsNullOrEmpty(comboBoxWebScript.Text))
                {
                    File.Delete(pathFile);
                    comboBoxWebScript.Text = string.Empty;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка удаления файла!");
            }
            GetFiles();
        }

        private void buttonSendContext_Click(object sender, EventArgs e)
        {
            if (Config.ServerTitleContext == string.Empty)
            {
                MessageBox.Show("Пустое поле для Context");
                return;
            }
            if (checkedListBoxFilesInContext.CheckedItems.Count > 0)
            {
                string[] pathScripts = new string[checkedListBoxFilesInContext.CheckedItems.Count];
                List<string> methods = new List<string>();

                for (int i = 0; i < checkedListBoxFilesInContext.CheckedItems.Count; i++)
                {
                    pathScripts[i] = checkedListBoxFilesInContext.CheckedItems[i].ToString();
                }
                if (string.IsNullOrEmpty(textBoxKeyEncode.Text))
                {
                    MessageBox.Show("Введите нормальный ключ шифрования!");
                }
                else
                {
                    foreach (string nameScript in pathScripts)
                    {
                        string lines = nameScript + "@" + File.ReadAllText(Path.Combine(Config.DirectoryWebScripts, nameScript));
                        methods.Add(EncryptionHelper.Encrypt(lines, textBoxKeyEncode.Text));
                    }
                }


                TelegraphServer = new TelegraphServer(TelegraphServer.Access_Token);
                MessageBox.Show(TelegraphServer.EditPage(Config.ServerTitleContext, Config.ServerTitleContext, methods).ToString());

            }
        }
        private void buttonCompilator_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBoxWebScript.Text))
            {
                if (File.Exists("CompilerDefult.exe"))
                {
                    Process.Start("CompilerDefult.exe", Path.Combine(Config.DirectoryWebScripts, comboBoxWebScript.Text));
                }
                else
                {
                    MessageBox.Show("мы не нашли модуль(CompilerDefult.exe) компиляциия не будет начита");
                }
            }
        }
        private void buttonRefrehMethod_Click(object sender, EventArgs e)
        {
            comboBoxReadWebScript.Items.Clear();
            webScript = Web.SendGetMethod(Config.ServerTitleContext);
            foreach (string name in webScript.Keys)
            {
                comboBoxReadWebScript.Items.Add(name);
                comboBoxReadWebScript.Text = comboBoxReadWebScript.Items[0].ToString();
                Comand.Add(name);
            }
            GetComand();
        }
        private void textBoxKeyEncode_TextChanged(object sender, EventArgs e)
        {
            Config.SecretKey = textBoxKeyEncode.Text;
        }
        private void comboBoxReadWebScript_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBoxWebScriptsInput.Text = webScript[comboBoxReadWebScript.Text];
        }
        private void buttonCreteNewComand_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxComandNEwName.Text))
            {
                if (!comboBoxComand.Items.Contains(textBoxComandNEwName.Text))
                {
                    Comand.Add(textBoxComandNEwName.Text);
                    GetComand();
                }
            }
        }
        private void buttonDelitComand_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBoxComand.Text))
            {
                DelitComand(comboBoxComand.Text);
            }
        }

        private void buttonCompilatorStart_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBoxWebScript.Text))
            {
                if (File.Exists("CompilerAndStart.exe"))
                {
                    Process.Start("CompilerAndStart.exe", Path.Combine(Config.DirectoryWebScripts, comboBoxWebScript.Text));
                }
                else
                {
                    MessageBox.Show("мы не нашли модуль(CompilerAndStart.exe) компиляциия не будет начита");
                }
            }
        }

        private void comboBoxYserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxYserName.Text == "All")
            {
                textBoxWebName.Enabled = false;
                YserName = textBoxWebName.Text;
                textBoxWebName.Text = "All";
                labelWebName.Enabled = false;
            }
            else
            {
                labelWebName.Enabled = true;
                textBoxWebName.Text = YserName;
                textBoxWebName.Enabled = true;
            }

        }

        private void buttonRegOutputName_Click(object sender, EventArgs e)
        {
            string name = textBoxOutputName.Text;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Ошибка при регистрации");
            }
            else
            {
                TelegraphServer = new TelegraphServer();
                TelegraphServer.CreateAccount(name);
                if (string.IsNullOrEmpty(TelegraphServer.Access_Token))
                {
                    MessageBox.Show("Ошибка: акаунт не создан");
                }
                else
                {
                    textBoxAccessTokenOutput.Text = TelegraphServer.Access_Token;
                    MessageBox.Show("Акаунт создан!");
                }
            }
        }

        private void buttonSaveOutputToken_Click(object sender, EventArgs e)
        {
            File.WriteAllText(Config.AuthOutputFile, textBoxAccessTokenOutput.Text);
        }

        private void buttonActivationOutputToken_Click(object sender, EventArgs e)
        {
            string registrationAccessToken = textBoxAccessTokenOutput.Text;
            if (string.IsNullOrEmpty(registrationAccessToken))
            {
                MessageBox.Show("Ошибка при активации(токен)");
            }
            else
            {
                TelegraphServerOutput = new TelegraphServer(registrationAccessToken);
                buttonChangeTokenOutput.Enabled = true;
                buttonDeleteInfoPage.Enabled = true;
                buttonRefreh.Enabled = true;
                buttonCreteMethoodOnlines.Enabled = true;
            }
        }

        private void buttonChangeTokenOutput_Click(object sender, EventArgs e)
        {
            buttonChangeTokenOutput.Enabled = false;
            buttonDeleteInfoPage.Enabled = false;
            buttonRefreh.Enabled = false;
            buttonCreteMethoodOnlines.Enabled = false;
        }
        private void buttonRefreh_Click(object sender, EventArgs e)
        {
            listBoxPageOutput.Items.Clear();
            ContentOutputPages.Clear();
            richTextBoxOutputTextPage.Clear();
            TelegraphServerOutput = new TelegraphServer(TelegraphServerOutput.Access_Token);
            List<string> ListPath = TelegraphServerOutput.GetPageList();

            foreach (string path in ListPath)
            {
                List<string> infoPage = Web.SendGetDichoneryOn(path);
                Console.WriteLine();
                try
                {
                    if (EncryptionHelper.Decrypt(infoPage[0], Config.SecretKey).Split(Config.Spliter)[1] == "online")
                    {
                        string content = EncryptionHelper.Decrypt(infoPage[0], Config.SecretKey);
                        string name = content.Split(Config.Spliter)[0];
                        ContentOutputPages.Add(name, content);
                        listBoxPageOutput.Items.Add(name);
                    }
                }
                catch { }
            }
        }
        private void buttonDeleteInfoPage_Click(object sender, EventArgs e)
        {
            ContentOutputPages.Clear();
            listBoxPageOutput.Items.Clear();
            richTextBoxOutputTextPage.Clear();
            TelegraphServerOutput = new TelegraphServer(TelegraphServerOutput.Access_Token);
            List<string> ListPath = TelegraphServerOutput.GetPageList();

            foreach (string path in ListPath)
            {
                TelegraphServerOutput.EditPage(path, path.Split('-')[0], "Ofline");
            }
        }
        private void buttonCopyName_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(listBoxPageOutput.Text))
            {
                Clipboard.SetText(listBoxPageOutput.Text);
            }
        }
        private void buttonCreteMethoodOnlines_Click(object sender, EventArgs e)
        {
            try {
                if (File.Exists(Config.MethodDefinitionFile))
                {
                    if (!string.IsNullOrEmpty(File.ReadAllText(Config.MethodDefinitionFile)))
                    {
                        string content = File.ReadAllText(Config.MethodDefinitionFile);
                        if (content.Contains("TokenOutput82"))
                        {
                            content = content.Replace("TokenOutput82", textBoxAccessTokenOutput.Text);
                            File.WriteAllText(Path.Combine(Config.DirectoryWebScripts, "IsOnline"), content);
                            GetFiles();
                            MessageBox.Show("Новый скрипт создан под имением(IsOnline)");
                            return;
                        }
                    }
                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("Что-то не так со стоковым скриптом " + Config.MethodDefinitionFile + "\nВозможно он отцутсвует,проверти файлы");
        }

        private void listBoxPageOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(listBoxPageOutput.Text)) {
                richTextBoxOutputTextPage.Text = ContentOutputPages[listBoxPageOutput.Text];
            }
        }
    }
}
