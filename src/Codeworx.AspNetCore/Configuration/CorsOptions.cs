using System.Collections.Generic;

namespace Codeworx.AspNetCore.Configuration
{
    public class CorsOptions
    {
        public CorsOptions()
        {
            Origins = new List<string>();
            Methods = new List<string>() { "*" };
            Headers = new List<string>() { "*" };
        }

        public List<string> Headers { get; set; }

        public List<string> Methods { get; set; }

        public List<string> Origins { get; set; }
    }
}