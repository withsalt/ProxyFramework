﻿using System.IO;

namespace NetworkSocket.Streams
{
    /// <summary>
    /// 表示会话收到的数据流
    /// </summary>
    public class SessionStream : MemoryStream
    {
        /// <summary>
        /// 会话收到的数据流
        /// </summary>
        public SessionStream()
            : base()
        {
        }
    }
}
