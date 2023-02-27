using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.EmailCodeVerifyModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.EmailOTPCodeRepo
{
	public interface IEmailOTPCodeRepo : IRepository<TblEmailOtpcode>
    {
        Task<string> DeleteCode(string code);
    }
}

