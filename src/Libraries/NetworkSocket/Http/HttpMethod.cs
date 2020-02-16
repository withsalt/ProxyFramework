﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSocket.Http
{
    /// <summary>
    /// 表示请求方式   
    /// </summary>
    [Flags]
    public enum HttpMethod
    {
        /// <summary>
        /// Get
        /// </summary>
        GET = 0x1,

        /// <summary>
        /// Post
        /// </summary>
        POST = 0x2,

        /// <summary>
        /// PUT
        /// </summary>
        PUT = 0x4,

        /// <summary>
        /// DELETE
        /// </summary>
        DELETE = 0x8,
    }
}
