using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace ArizaKaydi
{
	public class DynamicClaimsMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IServiceProvider _serviceProvider;
		// Cache to track users whose claims have been recently updated
		private static readonly ConcurrentDictionary<string, DateTime> _recentlyUpdatedUsers = new();

		public DynamicClaimsMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
		{
			_next = next;
			_serviceProvider = serviceProvider;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Middleware'in sadece kimliği doğrulanmış kullanıcılar için çalışmasını sağla
			if (context.User?.Identity?.IsAuthenticated ?? false)
			{
				var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				// Kullanıcı ID'si var mı ve yenilenmesi gerekenler listesinde mi kontrol et
				if (userId != null && _recentlyUpdatedUsers.TryRemove(userId, out _)) // TryRemove hem kontrol eder hem siler
				{
					// Bu kullanıcının kimliği yenilenmeli.
					// ServiceProvider'dan gerekli servisleri al (her istekte yeni scope)
					using var scope = _serviceProvider.CreateScope();
					var userManager = scope.ServiceProvider.GetRequiredService<UserManager<panelUser>>();
					var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<panelUser>>();

					var user = await userManager.FindByIdAsync(userId);
					if (user != null)
					{
						// *** ÇÖZÜM: Kullanıcının oturumunu (ve çerezini) yenile ***
						await signInManager.RefreshSignInAsync(user);

						// `RefreshSignInAsync` zaten context.User'ı günceller, ancak
						// emin olmak için tekrar principal oluşturup atayabiliriz veya
						// RefreshSignInAsync'in context'i güncellediğine güvenebiliriz.
						// Genellikle RefreshSignInAsync sonrası context güncel olur,
						// ancak garantiye almak isterseniz:
						// context.User = await signInManager.CreateUserPrincipalAsync(user);

						// Artık kullanıcı listeden çıkarıldı (TryRemove sayesinde)
					}
					else
					{
						// Kullanıcı bulunamadıysa, listede kalmaması için tekrar dene
						_recentlyUpdatedUsers.TryRemove(userId, out _);
					}
				}
			}

			// Bir sonraki middleware'e geç
			await _next(context);
		}

		// Call this method when you update user claims/roles
		public static void MarkUserForRefresh(string userId)
		{
			_recentlyUpdatedUsers[userId] = DateTime.UtcNow;

			// Clean up old entries (older than 1 hour)
			var cutoff = DateTime.UtcNow.AddHours(-1);
			var expiredKeys = _recentlyUpdatedUsers
				.Where(kvp => kvp.Value < cutoff)
				.Select(kvp => kvp.Key)
				.ToList();

			foreach (var key in expiredKeys)
			{
				_recentlyUpdatedUsers.TryRemove(key, out _);
			}
		}
	}
}
