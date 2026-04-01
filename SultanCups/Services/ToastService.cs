namespace SultanCups.Services
{
    public class ToastService
    {
        // حدث يتم استدعاؤه عند طلب إظهار التنبيه
        public event Action<string, string>? OnShow;

        /// <summary>
        /// إظهار التنبيه
        /// </summary>
        /// <param name="message">نص الرسالة</param>
        /// <param name="cssClass">success للأخضر أو error للأحمر</param>
        public void ShowToast(string message, string cssClass = "success")
        {
            OnShow?.Invoke(message, cssClass);
        }
    }
}