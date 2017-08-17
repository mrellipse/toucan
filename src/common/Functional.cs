using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Toucan.Contract;

namespace Toucan.Common.Extensions
{
    public static class Functional
    {
        public static void Retry(this Action action, int maxAttempts, int delayInMilliseconds = 100, bool throwOnIncomplete = true)
        {
            int tries = 0;
            bool completed = false;
            Exception lastException = null;

            while (!completed && tries < maxAttempts)
            {
                try
                {
                    tries++;
                    action();
                    completed = true;
                    lastException = null;
                }
                catch (Exception e)
                {
                    lastException = e;
                }
                
                Task.Delay(delayInMilliseconds).Wait();
            }

            if (!completed && throwOnIncomplete && lastException != null)
                throw lastException;
        }
    }
}