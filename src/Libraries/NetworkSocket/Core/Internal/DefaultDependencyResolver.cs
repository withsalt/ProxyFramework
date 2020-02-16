﻿using System;

namespace NetworkSocket.Core
{
    /// <summary>
    /// 默认的依赖关系解析程序的实现
    /// </summary>
    class DefaultDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// 解析支持任意对象创建的一次注册的服务
        /// </summary>
        /// <param name="serviceType">所请求的服务或对象的类型</param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }

        /// <summary>
        /// 结束服务实例的生命
        /// </summary>
        /// <param name="service">服务实例</param>
        public void TerminateService(IDisposable service)
        {
            service.Dispose();
        }
    }
}
