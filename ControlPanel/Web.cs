using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;
using System.IO;
using System.Windows.Forms;

namespace ControlPanel
{
    internal class Web
    {
        private static string[] JsonClear(string jsonCode)
        {
            string clean = jsonCode.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("\"", "");
            return clean.Split(',');

        }
        public static string GetHTML(string uri)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;
                return wc.DownloadString(uri.Replace("api.", ""));
            }

        }
        public static string SendGet(string path)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri("https://api.telegra.ph/getPage/");
                HttpResponseMessage response = client.GetAsync(path + "?return_content=true").Result;
                response.EnsureSuccessStatusCode();

                string[] result_array = JsonClear(response.Content.ReadAsStringAsync().Result);

                string created_page = string.Empty;

                if (result_array[0] == "ok:true")
                {
                    for (int i = 0; i < result_array.Length; i++)
                    {
                        if (result_array[i].Contains("children"))
                        {
                            string[] tmp_array = result_array[i].Split(':');
                            created_page += tmp_array[tmp_array.Length - 1];
                            break;
                        }
                    }
                }
                return created_page;
            }
        }
        public static Dictionary<string, string> SendGetMethod(string path)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri("https://api.telegra.ph/getPage/");
                HttpResponseMessage response = client.GetAsync(path + "?return_content=true").Result;
                response.EnsureSuccessStatusCode();

                Dictionary<string, string> webMethod = new Dictionary<string, string>();

                string[] result_array = JsonClear(response.Content.ReadAsStringAsync().Result);

                if (result_array[0] == "ok:true")
                {
                    for (int i = 0; i < result_array.Length; i++)
                    {
                        if (result_array[i].Contains("children"))
                        {
                            try
                            {
                                string[] tmpArray = result_array[i].Split(':');
                                string tmpLines = tmpArray[tmpArray.Length - 1].Replace("\\", "").Trim();
                                webMethod.Add(EncryptionHelper.Decrypt(tmpLines, Config.SecretKey).Split('@')[0], "");
                                for (int g = 0; g < EncryptionHelper.Decrypt(tmpLines, Config.SecretKey).Split('@').Length; g++)
                                {
                                    if (g != 0)
                                    {
                                        webMethod[EncryptionHelper.Decrypt(tmpLines, Config.SecretKey).Split('@')[0]] += EncryptionHelper.Decrypt(tmpLines, Config.SecretKey).Split('@')[g];
                                        if (g != EncryptionHelper.Decrypt(tmpLines, Config.SecretKey).Split('@').Length - 1)
                                        {
                                            webMethod[EncryptionHelper.Decrypt(tmpLines, Config.SecretKey).Split('@')[0]] += "@";
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                MessageBox.Show("ошибка чтения методов");
                                webMethod.Clear();
                                return webMethod;
                            }
                        }
                    }
                }
                return webMethod;
            }
        }
        public static List<string> SendGetDichoneryOn(string path)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri("https://api.telegra.ph/getPage/");
                HttpResponseMessage response = client.GetAsync(path + "?return_content=true").Result;
                response.EnsureSuccessStatusCode();

                List<string> webMethod = new List<string>();

                string[] result_array = JsonClear(response.Content.ReadAsStringAsync().Result);

                if (result_array[0] == "ok:true")
                {
                    for (int i = 0; i < result_array.Length; i++)
                    {
                        if (result_array[i].Contains("children"))
                        {
                            try
                            {
                                string[] tmpArray = result_array[i].Split(':');
                                webMethod.Add(tmpArray[tmpArray.Length - 1]);
                            }
                            catch
                            {
                                MessageBox.Show("ошибка чтения методов");
                                webMethod.Clear();
                                return webMethod;
                            }
                        }
                    }
                }
                return webMethod;
            }
        }
        public static string SendPOST(string uri, string postData)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;
                return wc.UploadString(uri, "POST", postData);
            }
        }
    }
}
