using Entities.Common;
using Microsoft.Extensions.Configuration;

namespace Application.Points.Extentions
{
    public static class PointsConfiguration
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static int GetEarnPoints(EntityType entityType)
        {
            return GetPoints(entityType, "Earn");
        }

        public static int GetDeductPoints(EntityType entityType)
        {
            return GetPoints(entityType, "Consume");
        }

        private static int GetPoints(EntityType entityType, string action)
        {
            if (_configuration == null)
                throw new InvalidOperationException("PointsConfiguration not initialized!");

            string key = $"PointsConfiguration:{entityType}:{action}";
            int points = _configuration.GetValue<int>(key);

            // اگر مقدار وجود نداشت، از Default استفاده کن
            if (points == 0)
            {
                string defaultKey = $"PointsConfiguration:Default:{action}";
                points = _configuration.GetValue<int>(defaultKey);
            }

            return points;
        }
    }


}
