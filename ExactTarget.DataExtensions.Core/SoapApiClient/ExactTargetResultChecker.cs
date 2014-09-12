using System;
using System.Collections.Generic;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core.SoapApiClient
{
    public class ResultError
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class ExactTargetResultChecker
    {
        public static IEnumerable<ResultError> CheckResults(IEnumerable<Result> results)
        {
            var errors = new List<ResultError>();
            foreach (var result in results)
            {
                if (result == null)
                {
                    errors.Add(new ResultError {StatusMessage = "Unexpected null result from ET"});
                }
                if (result != null && !result.StatusCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
                {
                    errors.Add(new ResultError {StatusCode = result.StatusCode, StatusMessage = result.StatusMessage});
                }
            }
            return errors;
        }

        public static void CheckResult(Result result)
        {
            if (result == null)
            {
                throw new Exception("Received an unexpected null result from ExactTarget");
            }

            if (result.StatusCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            throw new Exception(string.Format("Received a Non OK result StatusCode:{0} StatusMessage:{1} ",
                result.StatusCode, result.StatusMessage));
        }
    }
}