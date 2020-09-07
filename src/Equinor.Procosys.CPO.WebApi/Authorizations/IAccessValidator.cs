﻿using System.Threading.Tasks;
using MediatR;

namespace Equinor.Procosys.CPO.WebApi.Authorizations
{
    public interface IAccessValidator
    {
        Task<bool> ValidateAsync<TRequest>(TRequest request) where TRequest: IBaseRequest;
    }
}
