using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthServer.Core.CommonDTOs
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }

        [JsonIgnore]
        public bool IsSuccessFull { get; private set; } /// <summary>
                                                        /// Kendi iç yapımızda kullanmak için. Data is Null veya statusCodedan yani businesskodu
                                                        /// yazıp başarılı olup olmadığını anlamamak için. JsonIgnore yaptık ki Client'a gitmesin.
                                                        /// </summary>

        public ErrorDto Error { get; private set; }



        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessFull = true };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccessFull = true };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T> { Error = errorDto, StatusCode = statusCode, IsSuccessFull = false };
        }

        public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);

            return new Response<T> { Error = errorDto, StatusCode = statusCode, IsSuccessFull = false };
        }
    }
}
