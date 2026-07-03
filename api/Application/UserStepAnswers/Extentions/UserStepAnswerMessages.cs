using System.Text;

namespace Application.UserStepAnswers.Extentions
{
    internal static class UserStepAnswerMessages
    {
        internal static string NewUserStepAnswerDescription(string text, string answerTitle)
        {
            StringBuilder message = new StringBuilder();

            // خط عنوان با اموجی
            message.AppendLine("💬 **پاسخ جدید دریافت شد!**");
            message.AppendLine();

            // بخش سوال با هایلایت
            message.AppendLine("📋 **سوال دوستت:**");
            message.AppendLine($"« {text} »");
            message.AppendLine();

            // بخش پاسخ با هایلایت
            message.AppendLine("✨ **پاسخ او:**");
            message.AppendLine($"« {answerTitle} »");
            message.AppendLine();

            // کال تو اکشن (Call to Action) با اموجی پویا
            message.AppendLine("🤝 **حالا نوبت توئه!**");
            message.Append("برو به صفحه چت و راهنمایی‌هایت را با او در میان بگذار ");
            message.Append("💌");

            return message.ToString();
        }

        internal static string NewUserStepAnswerSubject(string userFullName)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("👋 سلام قهرمان!");
            message.AppendLine();
            message.AppendLine($"🎯 **{userFullName}** دوست خوبت");
            message.AppendLine("تمرینش رو انجام داده 🏆");
            message.AppendLine();
            message.Append("بریم ببینیم چطور شده؟ 👀");

            return message.ToString();
        }
    }
}
