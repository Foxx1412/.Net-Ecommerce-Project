using System;
namespace Project_1.NewFolder1
{
    public class TimeHelper
    {
        private static readonly TimeZoneInfo VietnamTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        // Lấy thời gian hiện tại ở múi giờ Việt Nam
        public static DateTime NowVietnamTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }

        // Chuyển đổi từ UTC sang thời gian Việt Nam
        public static DateTime ConvertToVietnamTime(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
        }

        // Chuyển đổi từ thời gian Việt Nam sang UTC
        public static DateTime ConvertToUtc(DateTime vietnamDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(vietnamDateTime, VietnamTimeZone);
        }
    }
}
