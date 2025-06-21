// Case.Ui3/Services/ConversationStateService.cs
using System;

namespace Case.Ui3.Services
{
    public class ConversationStateService
    {
        private const string ConversationCookieName = "case_conversationId";
        private readonly ICookieService _cookieService;

        public ConversationStateService(ICookieService cookieService)
        {
            _cookieService = cookieService;
        }

        public string CurrentConversationId { get; private set; } = string.Empty;

        public event Action? OnChange;

        public void SetConversationId(string conversationId)
        {
            CurrentConversationId = conversationId;
            // Asynchronously save to cookie - fire and forget
            _ = SaveToCookieAsync(conversationId);
            NotifyStateChanged();
        }

        public async Task<string> GetConversationIdFromCookieAsync()
        {
            return await _cookieService.GetCookieAsync(ConversationCookieName);
        }

        private async Task SaveToCookieAsync(string conversationId)
        {
            if (!string.IsNullOrWhiteSpace(conversationId))
            {
                await _cookieService.SetCookieAsync(ConversationCookieName, conversationId);
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public async Task ClearConversationAsync()
        {
            CurrentConversationId = string.Empty;
            await _cookieService.DeleteCookieAsync(ConversationCookieName);
            NotifyStateChanged();
        }
    }
}
