using System.Windows;       // הספרייה של WPF
using SafeLearning.Data;    // כדי שנוכל להשתמש ב-DatabaseHelper

namespace SafeLearning
{
    // המחלקה הראשית של התוכנה
    public partial class App : Application
    {
        // פעולה שרצה אוטומטית כשהתוכנה נפתחת
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // מכינה את מסד הנתונים והטבלאות
                DatabaseHelper.Initialize();
            }
            catch (Exception ex)
            {
                // אם הייתה תקלה: מציגה הודעה וסוגרת את התוכנה
                MessageBox.Show($"לא ניתן לפתוח את מסד הנתונים:\n{ex.Message}", "שגיאה");
                Shutdown();
            }
        }
    }
}