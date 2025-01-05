using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ControlPanel
{
    internal class TelegraphServer
    {
        public string Access_Token { get; set; }

        private string api { get; } = "https://api.telegra.ph";

        public TelegraphServer() {
            Access_Token = string.Empty;
        }
        public TelegraphServer(string access_token)
        {
            Access_Token = access_token;
        }

        private string[] JsonClear(string jsonCode) {
            string clean = jsonCode.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("\"", "");
            return clean.Split(',');

        }
        public string CreateAccount(string name)
        {
            //https://api.telegra.ph/createAccount?short_name=Sandbox&author_name=Anonymous
            if (string.IsNullOrEmpty(name)) {
                return null;    
            }

            string result = Web.SendPOST($"{api}/createAccount",$"short_name={name}");

            string[] result_array = JsonClear(result);

            if (result_array[0] == "ok:true") { 
                for(int i = 0; i < result_array.Length; i++) {
                    if (result_array[i].Contains("access_token")) {
                        string[] tmp_array = result_array[i].Split(':');
                        Access_Token = tmp_array[tmp_array.Length - 1];
                        break;
                    }
                }
            }
            return Access_Token;
        }
        public List<string> GetPageList()
        {
            //https://api.telegra.ph/getPageList?access_token=d3b25feccb89e508a9114afb82aa421fe2a9712b963b387cc5ad71e58722

            string result = Web.SendPOST($"{api}/getPageList?", $"access_token={Access_Token}");

            string[] result_array = JsonClear(result);

            List<string> pages = new List<string>();

            if (result_array[0] == "ok:true")
            {
                for (int i = 0; i < result_array.Length; i++)
                {
                    if (result_array[i].Contains("path"))
                    {
                        string[] tmp_array = result_array[i].Split(':');
                        pages.Add(tmp_array[tmp_array.Length - 1]);
                    }
                }
            }
            else { 
            }
            return pages;
        }
        public string CreatePage(string title) {

            string result = Web.SendPOST($"{api}/createPage", $"access_token={Access_Token}&title={title}&content=[{{\"tag\":\"p\",\"children\":[\"0{{split}}0\"]}}]&return_content=false");              

            string[] result_array = JsonClear(result);

            string created_page = string.Empty;

            if (result_array[0] == "ok:true") {
                for (int i = 0; i < result_array.Length; i++)
                {
                    if (result_array[i].Contains("path"))
                    {
                        string[] tmp_array = result_array[i].Split(':');
                        created_page = tmp_array[tmp_array.Length - 1];
                        break;
                    }
                }
            }
            return created_page;
        }
        public bool EditPage(string PagePath ,string title,string content) {

            //https://api.telegra.ph/editPage/Sample-Page-12-15?access_token=d3b25feccb89e508a9114afb82aa421fe2a9712b963b387cc5ad71e58722&title=Sample+Page&author_name=Anonymous&content=[{"tag":"p","children":["Hello,+world!"]}]&return_content=true

            string result = Web.SendPOST($"{api}/editPage/{PagePath}", $"access_token={Access_Token}&title={title}&content=[{{\"tag\":\"p\",\"children\":[\"{content}\"]}}]&return_content=false");

            string[] result_array = JsonClear(result);

            if (result_array[0] != "ok:true")
            {
                MessageBox.Show(result);
                return false;
            }
            else {
                return true;
            }

        }
        public bool EditPage(string PagePath, string title, List<string> content)
        {
            //https://api.telegra.ph/editPage/Sample-Page-12-15?access_token=d3b25feccb89e508a9114afb82aa421fe2a9712b963b387cc5ad71e58722&title=Sample+Page&author_name=Anonymous&content=[{"tag":"p","children":["Hello,+world!"]}]&return_content=true
            string cont = string.Empty;
            foreach (string item in content) {
                if (content.IndexOf(item) != content.Count - 1)
                {
                    cont += $"{{\"tag\":\"p\",\"children\":[\"{item}\"]}},";
                }
                else { 
                    cont += $"{{\"tag\":\"p\",\"children\":[\"{item}\"]}}";
                }
                
            }

            string result = Web.SendPOST($"{api}/editPage/{PagePath}", $"access_token={Access_Token}&title={title}&content=[{cont}]&return_content=false");

            string[] result_array = JsonClear(result);

            if (result_array[0] != "ok:true")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }
}
