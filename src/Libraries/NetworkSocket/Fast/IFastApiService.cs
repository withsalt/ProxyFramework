﻿using System;
using System.Threading.Tasks;

namespace NetworkSocket.Fast
{
    /// <summary>
    /// 定义fast协议的Api服务
    /// </summary>
    public interface IFastApiService : IDisposable
    {
        /// <summary>
        /// 异步执行Api行为
        /// </summary>              
        /// <param name="actionContext">Api行为上下文</param>     
        /// <returns></returns>
        Task ExecuteAsync(ActionContext actionContext);
    }
}
