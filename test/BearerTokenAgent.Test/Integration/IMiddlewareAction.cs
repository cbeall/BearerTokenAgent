﻿using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BearerTokenAgent.Test.Integration
{
    public interface IMiddlewareAction
    {
        Task TakeAction(HttpContext context);
    }
}
