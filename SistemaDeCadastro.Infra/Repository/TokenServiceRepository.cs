using Microsoft.IdentityModel.Tokens;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;
using System;
<<<<<<< HEAD
<<<<<<< HEAD
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
=======
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
=======
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

<<<<<<< HEAD
<<<<<<< HEAD

=======
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
=======
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
namespace SistemaDeCadastro.Infra.Repository
{
    public class TokenServiceRepository : ITokenServiceRepository
    {
        public string GenerationToken(Usuario usuario)
        {
<<<<<<< HEAD
<<<<<<< HEAD

           
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
=======
=======
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
            //reivindicação para o nome do usuário
            Claim[] claims = new Claim[]
            {
                new Claim("username", usuario.UserName),
                new Claim("id", usuario.Id),
            };
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9ASHDA98H9ah9ha9H9A89n0f"));
            var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
<<<<<<< HEAD
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
=======
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
                (
                expires: DateTime.Now.AddMinutes(10),
                claims: claims,
                signingCredentials: signingCredentials
                 );
<<<<<<< HEAD
<<<<<<< HEAD

=======
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
=======
>>>>>>> dade35f4f33f7f2cc72204845271dbd3c91a527a
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
