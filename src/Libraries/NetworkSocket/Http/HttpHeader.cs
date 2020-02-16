﻿using NetworkSocket.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace NetworkSocket.Http
{
    /// <summary>
    /// 表示http头信息集合
    /// </summary>
    public class HttpHeader : HttpNameValueCollection
    {
        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T TryGet<T>(string key)
        {
            return this.TryGet<T>(key, default(T));
        }

        /// <summary>
        /// 获取指定键的值
        /// 失败则返回defaultValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public T TryGet<T>(string key, T defaultValue)
        {
            var value = this[key];
            if (string.IsNullOrEmpty(value) == true)
            {
                return defaultValue;
            }

            try
            {
                return Converter.Cast<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        } 
    }
}
