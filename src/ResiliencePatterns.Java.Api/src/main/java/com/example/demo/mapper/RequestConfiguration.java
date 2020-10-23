package com.example.demo.mapper;

public class RequestConfiguration {
	
	private Integer successRequests;
	private Integer maxRequests;
	private Integer timeout;
	public Integer getSuccessRequests() {
		return successRequests;
	}
	public void setSuccessRequests(Integer successRequests) {
		this.successRequests = successRequests;
	}
	public Integer getMaxRequests() {
		return maxRequests;
	}
	public void setMaxRequests(Integer maxRequests) {
		this.maxRequests = maxRequests;
	}
	public Integer getTimeout() {
		return timeout;
	}
	public void setTimeout(Integer timeout) {
		this.timeout = timeout;
	}
	

}
