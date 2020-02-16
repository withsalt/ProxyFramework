using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSocket.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpExceptionMessage
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 创建Http错误消息
        /// </summary>
        /// <param name="code"></param>
        public HttpExceptionMessage(int code)
        {
            SetMessageByCode(code, string.Empty);
        }

        /// <summary>
        /// 创建Http错误消息
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="message">错误消息</param>
        public HttpExceptionMessage(int code, string message)
        {
            SetMessageByCode(code, message);
        }

        private void SetMessageByCode(int code, string message)
        {
            switch (code)
            {
                case 400:
                    Message = _400_Error(message);
                    break;
                case 403:
                    Message = _403_Error(message);
                    break;
                case 404:
                    Message = _404_Error(message);
                    break;
                default:
                    Message = _400_Error(message);
                    break;
            }
        }

        private string _400_Error(string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                return "<!DOCTYPE HTML PUBLIC \" -//IETF//DTD HTML 2.0//EN\"><html><head><title>400 Bad Request</title></head><body><h1>Request Error</h1><p>Please check your request data.<br /></p></body></html>";
            }
            else
            {
                return $"<!DOCTYPE HTML PUBLIC \" -//IETF//DTD HTML 2.0//EN\"><html><head><title>400 Bad Request</title></head><body><h1>Request Error</h1><p>{message}<br /></p></body></html>";
            }
        }

        private string _403_Error(string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                return "<!DOCTYPE HTML PUBLIC \" -//IETF//DTD HTML 2.0//EN\"><html><head><title>403 Forbidden</title></head><body><h1>Forbidden</h1><p>You don't have permission to access this folder on this server.<br /></p></body></html>";

            }
            else
            {
                return $"<!DOCTYPE HTML PUBLIC \" -//IETF//DTD HTML 2.0//EN\"><html><head><title>403 Forbidden</title></head><body><h1>Forbidden</h1><p>{message}<br /></p></body></html>";
            }
        }

        private string _404_Error(string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                return "<!DOCTYPE HTML PUBLIC \" -//IETF//DTD HTML 2.0//EN\"><html><head><title>404 Not Found</title></head><body><h1>404 Not Found</h1><p>The requested resource is not available.<br /></p></body></html>";
            }
            else
            {
                return $"<!DOCTYPE HTML PUBLIC \" -//IETF//DTD HTML 2.0//EN\"><html><head><title>404 Not Found</title></head><body><h1>404 Not Found</h1><p>{message}<br /></p></body></html>";
            }

        }
    }
}
