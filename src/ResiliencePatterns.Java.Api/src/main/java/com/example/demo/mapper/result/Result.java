package com.example.demo.mapper.result;

public class Result {
	private Long totalTime;
	private ClientToModule clientToModule;
	private ResilienceModuleToExternalService resilienceModuleToExternalService;
	private RetryMetrics retryMetrics;
	private CircuitBreakerMetrics circuitBreakerMetrics;
	
	public Result() {
		this.totalTime = 0L;
		this.clientToModule = new ClientToModule();
		this.resilienceModuleToExternalService = new ResilienceModuleToExternalService();
		this.retryMetrics = new RetryMetrics();
		this.circuitBreakerMetrics = new CircuitBreakerMetrics();
	}
	
	public Long getTotalTime() {
		return totalTime;
	}
	public void setTotalTime(Long totalTime) {
		this.totalTime = totalTime;
	}
	public ClientToModule getClientToModule() {
		return clientToModule;
	}
	public void setClientToModule(ClientToModule clientToModule) {
		this.clientToModule = clientToModule;
	}
	public ResilienceModuleToExternalService getResilienceModuleToExternalService() {
		return resilienceModuleToExternalService;
	}
	public void setResilienceModuleToExternalService(ResilienceModuleToExternalService resilienceModuleToExternalService) {
		this.resilienceModuleToExternalService = resilienceModuleToExternalService;
	}
	public RetryMetrics getRetryMetrics() {
		return retryMetrics;
	}
	public void setRetryMetrics(RetryMetrics retryMetrics) {
		this.retryMetrics = retryMetrics;
	}
	public CircuitBreakerMetrics getCircuitBreakerMetrics() {
		return circuitBreakerMetrics;
	}
	public void setCircuitBreakerMetrics(CircuitBreakerMetrics circuitBreakerMetrics) {
		this.circuitBreakerMetrics = circuitBreakerMetrics;
	}
	
	

}
