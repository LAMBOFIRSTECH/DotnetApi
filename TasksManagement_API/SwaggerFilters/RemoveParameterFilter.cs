using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace TasksManagement_API.SwaggerFilters
{
	public class RemoveParameterFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var parameters = operation.Parameters;

			var parameterToRemove = parameters.FirstOrDefault(p => p.Name == "identifiant");
			if (parameterToRemove != null)
			{
				parameters.Remove(parameterToRemove);
			}	
		}
	}
}