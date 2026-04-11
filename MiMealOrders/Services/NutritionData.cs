using System;

namespace MiMealOrders.Services
{
    /// <summary>
    /// POCO model for AI-estimated nutrition data for a menu item.
    /// All nutritional fields are nullable — null means no lookup has been performed.
    /// </summary>
    public class NutritionData
    {
        public int? Calories { get; set; }
        public decimal? TotalFatG { get; set; }
        public decimal? SaturatedFatG { get; set; }
        public decimal? CholesterolMg { get; set; }
        public decimal? SodiumMg { get; set; }
        public decimal? TotalCarbG { get; set; }
        public decimal? DietaryFiberG { get; set; }
        public decimal? SugarsG { get; set; }
        public decimal? ProteinG { get; set; }

        /// <summary>True when values came from AI estimation rather than a verified source.</summary>
        public bool IsAIEstimate { get; set; }

        public DateTime LookedUpAt { get; set; }

        /// <summary>Non-null if the lookup failed. UI should display this message.</summary>
        public string ErrorMessage { get; set; }

        public bool HasError { get { return !string.IsNullOrEmpty(ErrorMessage); } }
    }
}
