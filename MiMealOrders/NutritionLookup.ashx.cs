using System;
using System.Web;
using System.Web.Script.Serialization;
using MiMealOrders.Services;

namespace MiMealOrders
{
    /// <summary>
    /// HTTP handler for AI nutrition lookups.
    /// Called via JavaScript XHR from MenuItemCreation.aspx.
    /// GET ?itemName=Breakfast+Taco
    /// Returns: JSON NutritionData object
    /// </summary>
    public class NutritionLookup : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            string itemName = context.Request.QueryString["itemName"];

            NutritionData data = NutritionService.Lookup(itemName);

            var serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(data));
        }

        public bool IsReusable { get { return false; } }
    }
}
