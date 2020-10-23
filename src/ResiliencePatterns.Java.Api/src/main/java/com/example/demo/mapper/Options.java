package com.example.demo.mapper;

public class Options {
	
	private UrlConfiguration urlConfiguration;
	private RequestConfiguration requestConfiguration;
	
	private String runPolicy;

	
	private CircuitBreakerConfiguration circuitBreakerConfiguration;
	
	private RetryConfiguration retryConfiguration;

	
	public RetryConfiguration getRetryConfiguration() {
		return retryConfiguration;
	}

	public void setRetryConfiguration(RetryConfiguration retryConfiguration) {
		this.retryConfiguration = retryConfiguration;
	}

	public UrlConfiguration getUrlConfiguration() {
		return urlConfiguration;
	}

	public void setUrlConfiguration(UrlConfiguration urlConfiguration) {
		this.urlConfiguration = urlConfiguration;
	}

	public RequestConfiguration getRequestConfiguration() {
		return requestConfiguration;
	}

	public void setRequestConfiguration(RequestConfiguration requestConfiguration) {
		this.requestConfiguration = requestConfiguration;
	}

	public CircuitBreakerConfiguration getCircuitBreakerConfiguration() {
		return circuitBreakerConfiguration;
	}

	public void setCircuitBreakerConfiguration(CircuitBreakerConfiguration circuitBreakerConfiguration) {
		this.circuitBreakerConfiguration = circuitBreakerConfiguration;
	}

	public String getRunPolicy() {
		return runPolicy;
	}

	public void setRunPolicy(String runPolicy) {
		this.runPolicy = runPolicy;
	}
	

}
