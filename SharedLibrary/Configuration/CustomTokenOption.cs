using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Configuration
{
    //Token dağıtan API'mızın appsettings.json'a koyduğumuz token bilgilerinin class versiyonunu
    //yazdık ordaki bilgileri otomatik olarak map edecek => Options Pattern
    //builder.Configuration.GetSection("TokenOption") Apideki program.cs'de bunu yazarak DI containere geçtik.
    //TokenService'de de nası alındığı var.
    public class CustomTokenOption
    {
        public List<string> Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public string SecurityKey { get; set; }
    }
}
