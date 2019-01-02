using System.Threading.Tasks;
using Koptcha.Models;

namespace Koptcha.Services
{
    public interface ICaptchaService
    {
        Task<bool> CheckCaptchaAsync(CaptchaParameters captchaParameters);
    }
}