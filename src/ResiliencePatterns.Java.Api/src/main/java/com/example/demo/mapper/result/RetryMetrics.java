package com.example.demo.mapper.result;

public class RetryMetrics {

	private Integer retryCount;
	private Long totalTimeout;
	
	public RetryMetrics() {
		retryCount = 0;
		totalTimeout = 0L;
	}
	
	public Integer getRetryCount() {
		return retryCount;
	}
	public void setRetryCount(Integer retryCount) {
		this.retryCount = retryCount;
	}
	public long getTotalTimeout() {
		return totalTimeout;
	}
	public void setTotalTimeout(long totalTimeout) {
		this.totalTimeout = totalTimeout;
	}

}
