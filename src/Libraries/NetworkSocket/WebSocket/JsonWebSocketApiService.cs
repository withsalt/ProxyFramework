﻿using NetworkSocket.Core;
using NetworkSocket.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkSocket.WebSocket
{
    /// <summary>
    /// 表示JsonWebsocket协议的Api服务基类
    /// </summary>
    public abstract class JsonWebSocketApiService : JsonWebSocketFilterAttribute, IJsonWebSocketApiService
    {
        /// <summary>
        /// 获取关联的服务器实例
        /// </summary>
        protected JsonWebSocketMiddleware Middleware { get; private set; }

        /// <summary>
        /// 获取当前Api行为上下文
        /// </summary>
        protected ActionContext CurrentContext { get; private set; }

        /// <summary>
        /// JsonWebsocket协议的Api服务基类
        /// </summary>
        public JsonWebSocketApiService()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="middleware">关联的中间件</param>
        /// <returns></returns>
        internal JsonWebSocketApiService Init(JsonWebSocketMiddleware middleware)
        {
            this.Middleware = middleware;
            return this;
        }

        /// <summary>
        /// 异步执行Api行为
        /// </summary>   
        /// <param name="actionContext">上下文</param>      
        async Task IJsonWebSocketApiService.ExecuteAsync(ActionContext actionContext)
        {
            var filters = Enumerable.Empty<IFilter>();
            try
            {
                this.CurrentContext = actionContext;
                filters = this.Middleware.FilterAttributeProvider.GetActionFilters(actionContext.Action);
                await this.ExecuteActionAsync(actionContext, filters);
            }
            catch (Exception ex)
            {
                this.ProcessExecutingException(actionContext, filters, ex);
            }
        }

        /// <summary>
        /// 处理Api行为执行过程中产生的异常
        /// </summary>
        /// <param name="actionContext">上下文</param>
        /// <param name="actionfilters">过滤器</param>
        /// <param name="exception">异常项</param>
        private void ProcessExecutingException(ActionContext actionContext, IEnumerable<IFilter> actionfilters, Exception exception)
        {
            var exceptionContext = new ExceptionContext(actionContext, new ApiExecuteException(exception));
            this.ExecAllExceptionFilters(actionfilters, exceptionContext);
            this.Middleware.SendRemoteException(exceptionContext, exceptionContext.Exception);
        }

        /// <summary>
        /// 调用自身实现的Api行为
        /// </summary>       
        /// <param name="actionContext">上下文</param>       
        /// <param name="filters">过滤器</param>
        /// <returns></returns>
        private async Task ExecuteActionAsync(ActionContext actionContext, IEnumerable<IFilter> filters)
        {
            this.SetParameterValues(actionContext);
            this.ExecFiltersBeforeAction(filters, actionContext);

            if (actionContext.Result != null)
            {
                var exceptionContext = new ExceptionContext(actionContext, actionContext.Result);
                this.Middleware.SendRemoteException(actionContext, actionContext.Result);
            }
            else
            {
                await this.ExecutingActionAsync(actionContext, filters);
            }
        }

        /// <summary>
        /// 异步执行Api
        /// </summary>
        /// <param name="actionContext">上下文</param>
        /// <param name="filters">过滤器</param>
        /// <returns></returns>
        private async Task ExecutingActionAsync(ActionContext actionContext, IEnumerable<IFilter> filters)
        {
            var parameters = actionContext.Action.Parameters.Select(p => p.Value).ToArray();
            var result = await actionContext.Action.ExecuteAsync(this, parameters);

            this.ExecFiltersAfterAction(filters, actionContext);
            if (actionContext.Result != null)
            {
                this.Middleware.SendRemoteException(actionContext, actionContext.Result);
            }
            else if (actionContext.Action.IsVoidReturn == false && actionContext.Session.IsConnected)  // 返回数据
            {
                actionContext.Packet.body = result;
                var packetJson = this.Middleware.JsonSerializer.Serialize(actionContext.Packet);
                actionContext.Session.UnWrap().SendText(packetJson);
            }
        }

        /// <summary>
        /// 设置Api行为的参数值
        /// </summary> 
        /// <param name="context">上下文</param>        
        /// <exception cref="ArgumentException"></exception>    
        private void SetParameterValues(ActionContext context)
        {
            var body = context.Packet.body as IList;
            if (body == null)
            {
                throw new ArgumentException("body参数必须为数组");
            }

            if (body.Count != context.Action.Parameters.Length)
            {
                throw new ArgumentException("body参数数量不正确");
            }

            var serializer = this.Middleware.JsonSerializer;
            for (var i = 0; i < context.Action.Parameters.Length; i++)
            {
                var parameter = context.Action.Parameters[i];
                var value = serializer.Convert(body[i], parameter.Type);
                parameter.Value = value;
            }
        }

        /// <summary>
        /// 在Api行为前 执行过滤器
        /// </summary>       
        /// <param name="filters">Api行为过滤器</param>
        /// <param name="actionContext">上下文</param>   
        private void ExecFiltersBeforeAction(IEnumerable<IFilter> filters, ActionContext actionContext)
        {
            var totalFilters = this.GetTotalFilters(filters);
            foreach (var filter in totalFilters)
            {
                filter.OnExecuting(actionContext);
                if (actionContext.Result != null) break;
            }
        }

        /// <summary>
        /// 在Api行为后执行过滤器
        /// </summary>       
        /// <param name="filters">Api行为过滤器</param>
        /// <param name="actionContext">上下文</param>       
        private void ExecFiltersAfterAction(IEnumerable<IFilter> filters, ActionContext actionContext)
        {
            var totalFilters = this.GetTotalFilters(filters);
            foreach (var filter in totalFilters)
            {
                filter.OnExecuted(actionContext);
                if (actionContext.Result != null) break;
            }
        }

        /// <summary>
        /// 执行异常过滤器
        /// </summary>       
        /// <param name="filters">Api行为过滤器</param>
        /// <param name="exceptionContext">上下文</param>       
        private void ExecAllExceptionFilters(IEnumerable<IFilter> filters, ExceptionContext exceptionContext)
        {
            var totalFilters = this.GetTotalFilters(filters);
            foreach (var filter in totalFilters)
            {
                filter.OnException(exceptionContext);
                if (exceptionContext.ExceptionHandled == true) break;
            }
        }

        /// <summary>
        /// 获取全部的过滤器
        /// </summary>
        /// <param name="filters">行为过滤器</param>
        /// <returns></returns>
        private IEnumerable<IFilter> GetTotalFilters(IEnumerable<IFilter> filters)
        {
            return this.Middleware
                .GlobalFilters
                .Cast<IFilter>()
                .Concat(new[] { this })
                .Concat(filters);
        }
        #region IDisponse
        /// <summary>
        /// 获取对象是否已释放
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 关闭和释放所有相关资源
        /// </summary>
        public void Dispose()
        {
            if (this.IsDisposed == false)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            this.IsDisposed = true;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~JsonWebSocketApiService()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion
    }
}
