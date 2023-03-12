using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.EmailOTPCodeRepo
{
    public interface IEmailOTPCodeRepo : IRepository<TblEmailOtpcode>
    {
        Task<string> DeleteCode(string code);
    }
}

