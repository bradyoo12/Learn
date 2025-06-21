// Case.Ui3/Services/CookieService.cs
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Case.Ui3.Services
{
    public interface ICookieService
    {
        Task<string> GetCookieAsync(string name);
        Task SetCookieAsync(string name, string value, int days = 30);
        Task DeleteCookieAsync(string name);
    }

    public class CookieService : ICookieService
    {
        private readonly IJSRuntime _jsRuntime;

        public CookieService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetCookieAsync(string name)
        {
            return await _jsRuntime.InvokeAsync<string>("getCookie", name);
        }

        public async Task SetCookieAsync(string name, string value, int days = 30)
        {
            await _jsRuntime.InvokeVoidAsync("setCookie", name, value, days);
        }

        public async Task DeleteCookieAsync(string name)
        {
            await _jsRuntime.InvokeVoidAsync("eraseCookie", name);
        }
    }
}
