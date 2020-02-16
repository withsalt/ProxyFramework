﻿using System;
using System.Diagnostics;

namespace NetworkSocket.Exceptions
{
    /// <summary>
    /// 表示远程端Api行为异常
    /// </summary>
    [DebuggerDisplay("Message = {Message}")]
    public class RemoteException : Exception
    {
        /// <summary>
        /// 远程端Api行为异常
        /// </summary>       
        /// <param name="message">异常信息</param>
        public RemoteException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 从string隐式转换
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <returns></returns>
        public static implicit operator RemoteException(string message)
        {
            return new RemoteException(message);
        }
    }
}
