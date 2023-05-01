using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOptions _TokenOptions;

        public TokenService(UserManager<UserApp> userManager,IOptions<CustomTokenOptions> options)
        {
            _userManager= userManager;
            _TokenOptions= options.Value;
        }
        private string CreateRefreshToken()
        {
            //return Guid.NewGuid().ToString(); //Buda bir seçenek
            var numberByte=new byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }

        //audiences ler Bu tokenın hangi apiye yada apilere istek yapacağına bakıyor
        private IEnumerable<Claim> GetClaims(UserApp userApp,List<String>audiences)
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
             //Select foeach gibi düşün herbirini dönüp yeni bir claim oluştur ve Aud ları ekle bunlara
            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
        }

        private IEnumerable<Claim>GetClaimsByClient(Client client)
        {
        var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
             new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());
            return claims;

        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_TokenOptions.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_TokenOptions.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_TokenOptions.SecurityKey);// Tokenı imzalayacak security yekimiz
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _TokenOptions.Issuer, //Tokını yayınlayan kim?
                expires: accessTokenExpiration, //xzamanaı
                notBefore: DateTime.Now,       //Şu saatten itibaren olmasın ,yada o zaman içerisinde geçerli olun
                claims: GetClaims(userApp, _TokenOptions.Audience), //Git Claimlerini oluştur
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken); //oluşan Token =>3 parçadan oluşdu

            var tokenDto = new TokenDto //gelen tokını Token Dto ya dönüştürmen gerekiyor
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            return tokenDto;

        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_TokenOptions.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_TokenOptions.SecurityKey);// Tokenı imzalayacak security yekimiz
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _TokenOptions.Issuer, 
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,     
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken); 

            var tokenDto = new ClientTokenDto 
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
            };
            return tokenDto;
        }
    }
}
