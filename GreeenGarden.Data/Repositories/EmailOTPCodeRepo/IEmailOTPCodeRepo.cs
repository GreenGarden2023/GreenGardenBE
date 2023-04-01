using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.EmailOTPCodeRepo
{
    public interface IEmailOTPCodeRepo : IRepository<TblEmailOtpcode>
    {
        Task<string> DeleteCode(string email, string code);
    }
}

