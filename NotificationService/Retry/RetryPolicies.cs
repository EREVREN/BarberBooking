using Polly;
using Polly.Retry;

namespace NotificationService.Retry;

public static class RetryPolicies
{
    public static AsyncRetryPolicy GetEmailRetryPolicy()
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                retry => TimeSpan.FromSeconds(2));
    }
   }

