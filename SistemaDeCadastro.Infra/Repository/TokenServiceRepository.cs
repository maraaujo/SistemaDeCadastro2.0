using Microsoft.IdentityModel.Tokens;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace SistemaDeCadastro.Infra.Repository
{
    public class TokenServiceRepository : ITokenServiceRepository
    {
        public string GenerationToken(Usuario usuario)
        {

           
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, usuario.UserName),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id),
                
            };

            
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuaChaveDeAssinaturaCom256BitsAqui!"));

            
            var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            
            var token = new JwtSecurityToken
            (
                expires: DateTime.Now.AddMinutes(60),
                claims: claims,
                signingCredentials: signingCredentials
            );


     
            Claim[] claim = new Claim[]
            {
                new Claim("username", usuario.UserName),
                new Claim("id", usuario.Id),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9ASHDA98H9ah9ha9H9A89n0f"));
            var signingCredential = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var toke = new JwtSecurityToken
                (
                expires: DateTime.Now.AddMinutes(10),
                claims: claims,
                signingCredentials: signingCredentials
                 );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
