using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksManagement_API.Interfaces
{
    public interface IRemoveParametersIn
    {
        Task<string> AccessToken();
        Task<bool> UsersManagement(List<string> queryParamsToRemove);
    }
}