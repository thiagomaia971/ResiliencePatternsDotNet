package com.example.demo;

import org.springframework.stereotype.Component;

@Component
public class GlobalVariables {
	
	public Integer failRequests;
	public Integer circuitBreakerFBCount;
	
	public Integer requestsToServer;
	public Integer successRequests;
	
	public GlobalVariables() {
		this.failRequests = 0;
		this.circuitBreakerFBCount = 0;
		this.requestsToServer = 0;
		this.successRequests = 0;
	}
}
