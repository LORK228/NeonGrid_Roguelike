using System;

namespace UI
{
    // Глобальная статическая шина для системных уведомлений
    public static class UIEventBus
    {
        public static event Action<string> OnNotification;
        public static void ShowText(string text) => OnNotification?.Invoke(text);
    }
}