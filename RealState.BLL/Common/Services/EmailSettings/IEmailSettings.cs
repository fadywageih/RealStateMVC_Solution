using RealState.DAL.Models.Identity;

namespace RealState.BLL.Common.Services.EmailSettings
{
    public interface IEmailSettings
    {
        public void SendEmail(Email email);
    }
}
