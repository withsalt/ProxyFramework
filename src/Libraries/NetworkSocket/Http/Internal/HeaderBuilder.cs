﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkSocket.Http
{
    /// <summary>
    /// 表示http头生成器
    /// </summary>
    class HeaderBuilder
    {
        /// <summary>
        /// 换行
        /// </summary>
        private static readonly string CRLF = "\r\n";

        /// <summary>
        /// hashSet
        /// </summary>
        private readonly HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// builder
        /// </summary>
        private readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// http头生成器
        /// </summary>
        private HeaderBuilder()
        {
        }

        /// <summary>
        /// 生成http回复头
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="statusDescription">说明</param>
        public static HeaderBuilder NewResonse(int status, string statusDescription)
        {
            var header = new HeaderBuilder();
            header.builder.AppendFormat("HTTP/1.1 {0} {1}", status, statusDescription).Append(CRLF);
            return header;
        }

        /// <summary>
        /// 生成http请求头
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="path">路径</param>
        public static HeaderBuilder NewRequest(HttpMethod method, string path)
        {
            var header = new HeaderBuilder();
            header.builder.AppendFormat("{0} {1} HTTP/1.1", method, path).Append(CRLF);
            return header;
        }

        /// <summary>
        /// 添加头
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Add(string key, object value)
        {
            if (this.hashSet.Add(key) == true)
            {
                this.builder.AppendFormat("{0}: {1}", key, value).Append(CRLF);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.builder.ToString() + HeaderBuilder.CRLF;
        }

        /// <summary>
        /// 转换为字节组
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(this.ToString());
        }
    }
}
