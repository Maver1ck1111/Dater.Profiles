using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiles.Application
{
    public class Result<T>
    {
        public T Value { get; private set; }
        public int StatusCode { get; private set; }
        public string ErrorMessage { get; private set; }

        public static Result<T> Success(T value, int statusCode = 200)
        {
            return new Result<T>
            {
                Value = value,
                StatusCode = statusCode
            };
        }
        public static Result<T> Failure(int statusCode, string errorMessage, T value = default)
        {
            return new Result<T>
            {
                Value = value,
                StatusCode = statusCode,
                ErrorMessage = errorMessage
            };
        }
    }
}
