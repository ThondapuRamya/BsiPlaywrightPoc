using TechTalk.SpecFlow;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class RetryHooks(ScenarioContext scenarioContext, ILogger logger)
    {
        private static readonly ConcurrentDictionary<string, int> RetryCounts = new();
        private int _maxRetryCount; // Default to no retry

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Look for a Retry tag in the format @Retry(n)
            foreach (var tag in scenarioContext.ScenarioInfo.Tags)
            {
                if (tag.StartsWith("Retry(") && tag.EndsWith(")"))
                {
                    var retryValue = tag.Substring(6, tag.Length - 7);
                    if (int.TryParse(retryValue, out var retries))
                    {
                        _maxRetryCount = retries;
                    }
                }
            }

            // Initialize retry count in the static dictionary if not already present
            var scenarioKey = GetScenarioKey();
            RetryCounts.TryAdd(scenarioKey, 0);
        }

        [AfterStep]
        public void AfterStep()
        {
            // If there is no retry setup, do nothing
            if (_maxRetryCount == 0) return;

            // If the test failed in any step, check if we can retry
            if (scenarioContext.TestError != null)
            {
                HandleRetry();
            }
        }

        //[AfterScenario]
        public void AfterScenarioCleanup()
        {
            // Handle hook-level failures (like BeforeScenario)
            if (scenarioContext.TestError != null)
            {
                HandleRetry();
            }

            // Clean up retry count after the last retry attempt
            var scenarioKey = GetScenarioKey();
            var currentRetryCount = RetryCounts[scenarioKey];

            // Only remove the retry count once all retry attempts have been exhausted
            if (currentRetryCount >= _maxRetryCount)
            {
                RetryCounts.TryRemove(scenarioKey, out _);
            }
        }

        private void HandleRetry()
        {
            var scenarioKey = GetScenarioKey();
            var currentRetryCount = RetryCounts[scenarioKey];

            if (currentRetryCount < _maxRetryCount)
            {
                // Increment the retry count
                currentRetryCount++;
                RetryCounts[scenarioKey] = currentRetryCount;

                // Log the retry attempt
                logger.LogInformation($"Retrying scenario: {scenarioContext.ScenarioInfo.Title}. Retry attempt {currentRetryCount}/{_maxRetryCount}");

                // Force a retry, except on the last attempt
                if (currentRetryCount < _maxRetryCount)
                {
                    const string forceScenarioRetryBasedOnAssertion = "force retry due to failure in scenario or hook";
                    forceScenarioRetryBasedOnAssertion.Should().Be("Scenario passed");
                }
            }
        }

        private string GetScenarioKey()
        {
            // Use the scenario title and tags as a unique key for retry tracking
            var scenarioTitle = scenarioContext.ScenarioInfo.Title;
            var scenarioTags = string.Join(",", scenarioContext.ScenarioInfo.Tags);
            return $"{scenarioTitle}_{scenarioTags}";
        }
    }
}
