using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore5.Contracts.V1.Requests
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
