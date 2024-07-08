using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShareClassLibrary.Dtos;
using static ShareClassLibrary.Dtos.ServiceResponses;

namespace ShareClassLibrary.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccountAsync(UserDto usserDto);
        Task<LoginResponse> LoginAccountAsync(LoginDto usserDto);
    }
}