# Input Client Configuration
```json
{
    "UrlConfiguration": {
        "BaseUrl": string,
        "Success": {
            "Url": string,
            "Method": "GET/POST/PUT"
        },
        "Error": {
            "Url": string,
            "Method": "GET/POST/PUT"
        }
    },
    "RequestConfiguration": {
        "SuccessRequests": int,
        "MaxRequests": int,
        "Delay": int,
        "ProbabilityError": int,
        "Timeout": int
    },
    "RunPolicy": "NONE/RETRY/CIRCUIT_BREAKER",
    "RetryConfiguration": {
        "Count": int,
        "SleepDuration": int,
        "SleepDurationType": "FIXED/EXPONENTIAL_BACKOFF"
    },
    "CircuitBreakerConfiguration": {
        "IsSimpleConfiguration": boolean,
        "DurationOfBreaking": int,
        "ExceptionsAllowedBeforeBreaking": int,
        "FailureThreshold": double,
        "SamplingDuration": int,
        "MinimumThroughput": int
    }
}
```

# Result Client Configuration
```json
{
    "TotalTime": "00h:00m:00s:000ms",
    "ClientToModule": {
        "Success": int,
        "Error": int,
        "Total": int
    },
    "ResilienceModuleToExternalService": {
        "Success": int,
        "Error": int,
        "Total": int
    },
    "RetryMetrics": {
        "RetryCount": int,
        "TotalTimeout": "00h:00m:00s:000ms"
    },
    "CircuitBreakerMetrics": {
        "BreakCount": int,
        "ResetStatCount": int,
        "TotalOfBreak": "00h:00m:00s:000ms"
    }
}
```

# Automatic Scenario Runner Configuration
```json
{
    "AutomaticRunnerConfiguration": {
        "ScenariosPath": "D:\\Wokspace\\Universidade\\ResiliencePatternsDotNet\\Scenarios"
    }
}
```

# Scenario Configuration
```json
{
    "Count": int,
    "UrlFetch": {
        "BaseUrl": string,
        "ActionUrl": string,
        "Method": "GET/POST/PUT"
    },
    "ResultType": "TXT/CSV",
    "Parameters": {
        "UrlConfiguration": {
            "BaseUrl": string,
            "Success": {
            "Url": string,
            "Method": "GET/POST/PUT"
            },
            "Error": {
            "Url": string,
            "Method": "GET/POST/PUT"
            }
        },
        "RequestConfiguration": {
            "SuccessRequests": int,
            "MaxRequests": int,
            "Delay": int,
            "ProbabilityError": int,
            "Timeout": int
        },
        "RunPolicy": "NONE/RETRY/CIRCUIT_BREAKER",
        "RetryConfiguration": {
            "Count": int,
            "SleepDuration": int,
            "SleepDurationType": "FIXED/EXPONENTIAL_BACKOFF"
        },
        "CircuitBreakerConfiguration": {
            "IsSimpleConfiguration": boolean,
            "DurationOfBreaking": int,
            "ExceptionsAllowedBeforeBreaking": int,
            "FailureThreshold": double,
            "SamplingDuration": int,
            "MinimumThroughput": int
        }
    }
}
```
