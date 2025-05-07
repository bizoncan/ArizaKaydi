using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArizaKaydi.Helper
{
	public static class HtmlExtensions
	{
		public static string IsActive(this IHtmlHelper htmlHelper, string controller, string action)
		{
			var routeData = htmlHelper.ViewContext?.RouteData;

			if (routeData == null ||
				!routeData.Values.ContainsKey("Controller") ||
				!routeData.Values.ContainsKey("Action"))
			{
				return "";
			}

			var currentController = routeData.Values["Controller"]?.ToString();
			var currentAction = routeData.Values["Action"]?.ToString();
			

			if (string.IsNullOrEmpty(currentController) || string.IsNullOrEmpty(currentAction))
			{
				return "";
			}

			if (string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase))
			{
				return "active";
			}
			else
			{
				return "";
			}

		}
	}
}
