using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;

namespace RigoFunc.OAuth {
    /// <summary>
    /// Represents the OAuth user.
    /// </summary>
    public class OAuthUser {
        /// <summary>
        /// Gets the OAuth user from <see cref="ClaimsPrincipal"/> identity user.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <returns>A new instance of <see cref="OAuthUser"/>.</returns>
        public static OAuthUser FromUser(ClaimsPrincipal principal) {
            return new OAuthUser {
                Id = int.Parse(principal.FindFirstValue(JwtClaimTypes.Subject)),
                UserName = principal.FindFirstValue(JwtClaimTypes.Name),
                PhoneNumber = principal.FindFirstValue(JwtClaimTypes.PhoneNumber),
                Email = principal.FindFirstValue(JwtClaimTypes.Email),
                RealName = principal.FindFirstValue(JwtClaimTypes.GivenName),
                NickName = principal.FindFirstValue(JwtClaimTypes.NickName),
                Gender = (Gender)Convert.ToInt32(principal.FindFirstValue(JwtClaimTypes.Gender)),
                BirthDate = Convert.ToDateTime(principal.FindFirstValue(JwtClaimTypes.BirthDate)),
                AvatarUrl = principal.FindFirstValue(JwtClaimTypes.Picture),
                QQ = principal.FindFirstValue(OAuthClaimTypes.QQ),
                Alipay = principal.FindFirstValue(OAuthClaimTypes.Alipay),
                WX = principal.FindFirstValue(OAuthClaimTypes.WX),
                WChat = principal.FindFirstValue(OAuthClaimTypes.WChat),
            };
        }

        /// <summary>
        /// To the claims.
        /// </summary>
        /// <returns>IEnumerable&lt;Claim&gt;.</returns>
        public IEnumerable<Claim> ToClaims() {
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(RealName)) {
                claims.Add(new Claim(JwtClaimTypes.GivenName, RealName));
            }
            if (!string.IsNullOrEmpty(NickName)) {
                claims.Add(new Claim(JwtClaimTypes.NickName, NickName));
            }
            if (!string.IsNullOrEmpty(AvatarUrl)) {
                claims.Add(new Claim(JwtClaimTypes.Picture, AvatarUrl));
            }
            if (!string.IsNullOrEmpty(QQ)) {
                claims.Add(new Claim(OAuthClaimTypes.QQ, QQ));
            }
            if (!string.IsNullOrEmpty(WX)) {
                claims.Add(new Claim(OAuthClaimTypes.WX, WX));
            }
            if (!string.IsNullOrEmpty(WChat)) {
                claims.Add(new Claim(OAuthClaimTypes.WChat, WChat ?? ""));
            }
            claims.Add(new Claim(JwtClaimTypes.Gender, ((int)Gender).ToString()));
            claims.Add(new Claim(JwtClaimTypes.BirthDate, BirthDate.ToString("yyyy-MM-dd hh:mm:ss")));

            return claims;
        }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户姓名.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 电话号码.
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 邮箱地址.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 真名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// 头像地址.
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 支付宝账号
        /// </summary>
        public string Alipay { get; set; }
        /// <summary>
        /// 微信支付账号
        /// </summary>
        public string WX { get; set; }
        /// <summary>
        /// 微信OpenId.
        /// </summary>
        public string WChat { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => UserName ?? PhoneNumber ?? base.ToString();
    }

    /// <summary>
    /// Represents the Gender enum.
    /// </summary>
    public enum Gender {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown,
        /// <summary>
        /// 男性
        /// </summary>
        Male,
        /// <summary>
        /// 女性
        /// </summary>
        Female,
    }

    /// <summary>
    /// Claims related extensions for <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class PrincipalExtensions {
        /// <summary>
        /// Returns the value for the first claim of the specified type otherwise null the claim is not present.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
        /// <param name="claimType">The claim type whose first value should be returned.</param>
        /// <returns>The value of the first instance of the specified claim type, or null if the claim is not present.</returns>
        public static string FindFirstValue(this ClaimsPrincipal principal, string claimType) {
            if (principal == null) {
                throw new ArgumentNullException(nameof(principal));
            }
            var claim = principal.FindFirst(claimType);
            return claim != null ? claim.Value : null;
        }
    }

    /// <summary>
    /// OAuth claim type names.
    /// </summary>
    public static class OAuthClaimTypes {
        /// <summary>
        /// QQ
        /// </summary>
        public const string QQ = "qq";
        /// <summary>
        /// Alipay
        /// </summary>
        public const string Alipay = "alipay";
        /// <summary>
        /// Weixin
        /// </summary>
        public const string WX = "wx";
        /// <summary>
        /// WChat
        /// </summary>
        public const string WChat = "wco";
    }
}
