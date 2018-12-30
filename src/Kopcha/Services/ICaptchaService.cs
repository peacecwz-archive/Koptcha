using System.Threading.Tasks;
using Kopcha.Models;

namespace Kopcha.Services
{
    public interface ICaptchaService
    {
        Task<bool> CheckCaptchaAsync(CaptchaParameters captchaParameters);
    }
}