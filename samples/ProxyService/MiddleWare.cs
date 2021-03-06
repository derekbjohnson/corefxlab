﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using ProxyService;

namespace Benchmarks.Middleware
{
    public class ProxyMiddleWare
    {

        private readonly RequestDelegate _next;
        HttpClient _client;

        public ProxyMiddleWare(RequestDelegate next)
        {
            _next = next;
            _client = new HttpClient();

        }

        public Task Invoke(HttpContext httpContext)
        {
            return ForwardToProxy(httpContext);
        }

        public async Task ForwardToProxy(HttpContext httpContext)
        {
            string path = httpContext.Request.Path.ToString();
            string outputUrl = Program.ForwardUrl + path;
            HttpResponseMessage response = await _client.GetAsync(outputUrl);

            // string body = await response.Content.ReadAsStringAsync();
            Stream stream = await response.Content.ReadAsStreamAsync();
            await stream.CopyToAsync(httpContext.Response.Body);
        }
    }

    public static class ProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseProxy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ProxyMiddleWare>();
        }
    }
}