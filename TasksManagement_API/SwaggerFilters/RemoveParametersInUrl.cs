using System.Web;
using Microsoft.AspNetCore.Http.Extensions;

namespace Tasks_WEB_API.SwaggerFilters
{
	public class RemoveParametersInUrl
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public RemoveParametersInUrl(IHttpContextAccessor httpContextAccessor)
		{

			this.httpContextAccessor = httpContextAccessor;
		}
		public async Task<string> EraseParametersInUri()
		{
			string requestUrl = httpContextAccessor.HttpContext!.Request.GetEncodedUrl();
			var uriBuilder = new UriBuilder(requestUrl);
			Console.WriteLine(uriBuilder);
			try
			{
				// Supprimer les paramètres dont vous ne voulez pas dans votre URL
				var queryParamsToRemove = new List<string> { "nom", "mdp", "role", "email" };
				var query = HttpUtility.ParseQueryString(uriBuilder.Query);
				Console.WriteLine(query);
				foreach (var param in queryParamsToRemove)
				{
					query.Remove(param);
				}
				uriBuilder.Query = query.ToString();
				Console.WriteLine("supprimé ici" + uriBuilder.Query);

				// Utilisez la nouvelle URL construite sans les paramètres
				var newUrl = uriBuilder.Uri.ToString();
				Console.WriteLine(newUrl);
				Console.WriteLine("----------------------------------");

				// Envoyez votre requête POST à la nouvelle URL
				using var httpClient = new HttpClient();
				var response = await httpClient.PostAsync(newUrl, null);
				Console.WriteLine(response.RequestMessage!.RequestUri);
				response.EnsureSuccessStatusCode();

				return newUrl;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Une erreur est survenue lors de la suppression des paramètres de l'URL {uriBuilder} : {ex.Message}");
				throw; // Renvoyer l'exception pour la gérer à un niveau supérieur si nécessaire
			}
		}
	}
}
