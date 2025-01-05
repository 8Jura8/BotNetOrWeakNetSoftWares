using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minishion2
{
    internal class Config
    {
        public static string SecretKey
        {
            get { 
                return "SwP9sph23b36cM"; 
            }
        }
        public static char Spliter
        {
            get
            {
                return '#';
            }
        }

        public static string WebContent
        {
            get
            {
                return "Context-12-29-6";
            }
        }

        public static string Comand
        {
            get { 
                return "Comand-12-29-8";
            }
        }

        public static string PathToId
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), "Jf23jgfjlk12fnjajF4IPOQP\\id.txt");
            }
        }
        public static string PathDirectoryToId
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), "Jf23jgfjlk12fnjajF4IPOQP");
            }
        }

        public static string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        private static string _Id;

        public static string YourTitle
        {
            get
            {
                return _YourTitle;
            }
            set
            {
                _YourTitle = value;
            }
        }
        private static string _YourTitle;

    }
}
