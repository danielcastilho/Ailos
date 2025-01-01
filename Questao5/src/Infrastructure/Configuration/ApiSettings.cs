using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ailos.Infrastructure.Configuration
{
    public class ApiSettings
    {
        public int Port { get; set; }
        public bool UseHttps { get; set; }
        public int HttpsPort { get; set; }
        public bool SwaggerEnabled { get; set; }
        public string SwaggerRoute { get; set; } = string.Empty;
    }
}
