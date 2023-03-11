using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.EmailOTPCodeRepo
{
    public class EmailOTPCodeRepo : Repository<TblEmailOtpcode>, IEmailOTPCodeRepo
    {
        private readonly GreenGardenDbContext _context;
        public EmailOTPCodeRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<string> DeleteCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                TblEmailOtpcode? emailCode = await _context.TblEmailOtpcodes.Where(x => x.Optcode.Equals(code)).FirstOrDefaultAsync();
                if (emailCode != null)
                {
                    _ = _context.TblEmailOtpcodes.Remove(emailCode);
                    await Update();
                    return emailCode.Email;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }

        }
    }
}

