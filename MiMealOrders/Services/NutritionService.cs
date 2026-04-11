using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace MiMealOrders.Services
{
    /// <summary>
    /// Looks up AI-estimated nutrition data for a food item name.
    /// Results are cached in HttpRuntime.Cache for 24 hours to avoid redundant API calls.
    /// Falls back to a structured stub when no API key is configured.
    /// </summary>
    public static class NutritionService
    {
        private const string OpenAIEndpoint = "https://api.openai.com/v1/chat/completions";
        private const int CacheHours = 24;

        private static readonly string SystemPrompt =
            "You are a nutrition database assistant. When given a school cafeteria food item name, " +
            "return ONLY a valid JSON object with these exact fields: " +
            "calories (integer), totalFatG (number), saturatedFatG (number), cholesterolMg (number), " +
            "sodiumMg (number), totalCarbG (number), dietaryFiberG (number), sugarsG (number), proteinG (number). " +
            "Use typical serving size for school meal portions. Return nothing except the JSON object.";

        /// <summary>
        /// Returns nutrition data for the given item name.
        /// Checks the in-process cache first; calls OpenAI if not cached.
        /// </summary>
        public static NutritionData Lookup(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                return new NutritionData { ErrorMessage = "Item name is required." };

            string cacheKey = "nutrition_" + itemName.ToLower().Trim();

            var cached = HttpRuntime.Cache[cacheKey] as NutritionData;
            if (cached != null)
                return cached;

            string apiKey = ConfigurationManager.AppSettings["OpenAIApiKey"];
            string model = ConfigurationManager.AppSettings["OpenAIModel"] ?? "gpt-4o-mini";

            NutritionData result;

            if (string.IsNullOrWhiteSpace(apiKey))
                result = BuildStub(itemName);
            else
                result = CallOpenAI(itemName, apiKey, model);

            if (!result.HasError)
            {
                HttpRuntime.Cache.Insert(
                    cacheKey,
                    result,
                    null,
                    DateTime.Now.AddHours(CacheHours),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return result;
        }

        private static NutritionData CallOpenAI(string itemName, string apiKey, string model)
        {
            try
            {
                var serializer = new JavaScriptSerializer();

                var requestBody = new Dictionary<string, object>
                {
                    { "model", model },
                    { "messages", new object[]
                        {
                            new Dictionary<string, string>
                            {
                                { "role", "system" },
                                { "content", SystemPrompt }
                            },
                            new Dictionary<string, string>
                            {
                                { "role", "user" },
                                { "content", "Food item: " + itemName }
                            }
                        }
                    },
                    { "temperature", 0.1 },
                    { "max_tokens", 200 }
                };

                string requestJson = serializer.Serialize(requestBody);
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestJson);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(OpenAIEndpoint);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + apiKey);
                request.ContentLength = requestBytes.Length;

                using (Stream stream = request.GetRequestStream())
                    stream.Write(requestBytes, 0, requestBytes.Length);

                string responseJson;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    responseJson = reader.ReadToEnd();

                dynamic responseObj = serializer.DeserializeObject(responseJson);
                string content = ((dynamic)((object[])((Dictionary<string, object>)responseObj)["choices"])[0])["message"]["content"].ToString();

                return ParseNutritionJson(content.Trim());
            }
            catch (WebException ex)
            {
                string errorDetail = ex.Message;
                if (ex.Response != null)
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                        errorDetail = reader.ReadToEnd();
                }
                return new NutritionData { ErrorMessage = "API error: " + errorDetail };
            }
            catch (Exception ex)
            {
                return new NutritionData { ErrorMessage = "Lookup failed: " + ex.Message };
            }
        }

        private static NutritionData ParseNutritionJson(string json)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                var dict = serializer.Deserialize<Dictionary<string, object>>(json);

                return new NutritionData
                {
                    Calories       = GetInt(dict, "calories"),
                    TotalFatG      = GetDecimal(dict, "totalFatG"),
                    SaturatedFatG  = GetDecimal(dict, "saturatedFatG"),
                    CholesterolMg  = GetDecimal(dict, "cholesterolMg"),
                    SodiumMg       = GetDecimal(dict, "sodiumMg"),
                    TotalCarbG     = GetDecimal(dict, "totalCarbG"),
                    DietaryFiberG  = GetDecimal(dict, "dietaryFiberG"),
                    SugarsG        = GetDecimal(dict, "sugarsG"),
                    ProteinG       = GetDecimal(dict, "proteinG"),
                    IsAIEstimate   = true,
                    LookedUpAt     = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                return new NutritionData { ErrorMessage = "Failed to parse nutrition response: " + ex.Message };
            }
        }

        /// <summary>
        /// Returns plausible stub nutrition data when no API key is configured.
        /// Values are generic school-lunch estimates suitable for demo purposes.
        /// </summary>
        private static NutritionData BuildStub(string itemName)
        {
            return new NutritionData
            {
                Calories      = 320,
                TotalFatG     = 9.0m,
                SaturatedFatG = 2.5m,
                CholesterolMg = 35m,
                SodiumMg      = 680m,
                TotalCarbG    = 44m,
                DietaryFiberG = 2m,
                SugarsG       = 8m,
                ProteinG      = 14m,
                IsAIEstimate  = true,
                LookedUpAt    = DateTime.Now
            };
        }

        private static int? GetInt(Dictionary<string, object> dict, string key)
        {
            if (!dict.ContainsKey(key) || dict[key] == null) return null;
            int val;
            if (int.TryParse(dict[key].ToString(), out val)) return val;
            return null;
        }

        private static decimal? GetDecimal(Dictionary<string, object> dict, string key)
        {
            if (!dict.ContainsKey(key) || dict[key] == null) return null;
            decimal val;
            if (decimal.TryParse(dict[key].ToString(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out val)) return val;
            return null;
        }
    }
}
