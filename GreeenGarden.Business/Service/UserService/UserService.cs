using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.UserRepo;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GreeenGarden.Business.Service.UserService
{
    
	public class UserService : IUserService
	{
        private readonly IUserRepo _userRepo;
        private readonly DecodeToken _decodeToken;

        public UserService(IUserRepo userRepo)
		{
            _userRepo = userRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<UserLoginResModel> Login(string userName)
        {
            try
            {
                UserLoginResModel userModel = await _userRepo.GetUser(userName);
                return userModel;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.ToString());
            }
        }

        public async Task<string> Register(UserInsertModel userInsertModel)
        {
            try
            {
                CreatePasswordHash(userInsertModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
                TblUser userModel = new TblUser()
                {
                    UserName = userInsertModel.UserName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FullName = userInsertModel.FullName,
                    Address = userInsertModel.Address,
                    Phone = userInsertModel.Phone,
                    Favorite = userInsertModel.Favorite,
                    Mail = userInsertModel.Mail,
                };
                await _userRepo.Insert(userModel);
                return "User registered!!!";

            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.ToString());
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        public string CreateToken(UserLoginResModel user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.RoleName),

            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                SecretService.SecretService.GetTokenSecret()));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<UserCurrResModel> GetCurrentUser(string token)
        {
            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            if (!userRole.Equals(Commons.ADMIN)
                && !userRole.Equals(Commons.CUSTOMER)
                && !userRole.Equals(Commons.MANAGER)
                && !userRole.Equals(Commons.STAFF)
                && !userRole.Equals(Commons.DELIVERER)) { throw new ArgumentException("Forbidden: You don't have permission to access this resource"); }
            try
            {
                string userName = _decodeToken.Decode(token, ClaimsIdentity.DefaultNameClaimType);
                UserCurrResModel userCurrResModel = await _userRepo.GetCurrentUser(userName);
                return userCurrResModel;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.ToString());
            }
        }
    }
}

