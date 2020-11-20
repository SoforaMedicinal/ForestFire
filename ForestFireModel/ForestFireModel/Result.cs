using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestFireModel
{
    public class Result<TResult>
    {
        protected Result(bool succeeded, IEnumerable<string> errors, TResult value)
        {
            Succeeded = succeeded;
            Value = value;
            if (errors == null)
            {
                errors = new string[0];
            }
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; protected set; }
        public IEnumerable<string> Errors { get; protected set; }
        public TResult Value { get; protected set; }

        public static Result<TResult> Success(TResult result)
        {
            return new Result<TResult>(true, null, result);
        }
        public static Result<TResult> Fail(params string[] errors)
        {
            return new Result<TResult>(false, errors, default);
        }
        public static Result<TResult> Fail(IEnumerable<string> errors)
        {
            return new Result<TResult>(false, errors, default);
        }
    }
}
