using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianKaiHua.Auxiliary
{
    public static class AccountHash
    {
        /// <summary>
        /// 获取用于加密 Token 的密钥
        /// </summary>
        /// <returns></returns>
        public static SigningCredentials GetTokenSecurityKey()
        {
            string SecretKey = Appsettings.app(new string[] { "Audience", "Secret" });
            var securityKey = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)), SecurityAlgorithms.HmacSha256);
            return securityKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SymmetricSecurityKey GetSecurityKey()
        {
            string SecretKey = Appsettings.app(new string[] { "Audience", "Secret" });
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            return securityKey;
        }
    }

}
