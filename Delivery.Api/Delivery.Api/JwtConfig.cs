using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.Api
{
    public class JwtConfig
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SigningKey { get; set; }

        public int Expires { get; set; }

        public bool ValidateLifetime { get; set; }
    }
}
