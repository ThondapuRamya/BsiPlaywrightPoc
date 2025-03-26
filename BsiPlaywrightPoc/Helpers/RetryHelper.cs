namespace BsiPlaywrightPoc.Helpers
{
    public static class RetryHelper
    {
        public static async Task Retry(Func<Task> action, int maxRetries = 3, int delayMilliseconds = 500)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    await action();
                    return;
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries)
                        throw;

                    Console.WriteLine($"Retry {attempt} failed: {ex.Message}");
                    await Task.Delay(delayMilliseconds);
                }
            }
        }

    }
}
