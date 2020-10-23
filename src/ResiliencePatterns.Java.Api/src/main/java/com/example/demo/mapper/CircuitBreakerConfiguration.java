package com.example.demo.mapper;

public class CircuitBreakerConfiguration {
	private Float failureThreshold = 50.0f; //failureRateThreshold
	private Integer slidingWindowSize = 100;
	private Integer minimumNumberOfCalls = 100;
	private Integer durationOfBreaking = 60000; //waitDurationInOpenState [ms]
	private Integer permittedNumberOfCallsInHalfOpenState = 10;
	private Integer slowCallRateThreshold = 100;
	private Integer slowCallDurationThreshold = 60000; //[ms]
	
	//ignoreExceptions, recordException, ignoreException, automaticTransitionFromOpenToHalfOpenEnabled, slidingWindowType, maxWaitDurationInHalfOpenState
	
	public Integer getDurationOfBreaking() {
		return durationOfBreaking;
	}
	public void setDurationOfBreaking(Integer durationOfBreaking) {
		this.durationOfBreaking = durationOfBreaking;
	}
	public Float getFailureThreshold() {
		return failureThreshold;
	}
	public void setFailureThreshold(Float failureThreshold) {
		this.failureThreshold = failureThreshold;
	}
	public Integer getSlowCallRateThreshold() {
		return slowCallRateThreshold;
	}
	public void setSlowCallRateThreshold(Integer slowCallRateThreshold) {
		this.slowCallRateThreshold = slowCallRateThreshold;
	}
	public Integer getSlowCallDurationThreshold() {
		return slowCallDurationThreshold;
	}
	public void setSlowCallDurationThreshold(Integer slowCallDurationThreshold) {
		this.slowCallDurationThreshold = slowCallDurationThreshold;
	}
	public Integer getPermittedNumberOfCallsInHalfOpenState() {
		return permittedNumberOfCallsInHalfOpenState;
	}
	public void setPermittedNumberOfCallsInHalfOpenState(Integer permittedNumberOfCallsInHalfOpenState) {
		this.permittedNumberOfCallsInHalfOpenState = permittedNumberOfCallsInHalfOpenState;
	}
	public Integer getSlidingWindowSize() {
		return slidingWindowSize;
	}
	public void setSlidingWindowSize(Integer slidingWindowSize) {
		this.slidingWindowSize = slidingWindowSize;
	}
	public Integer getMinimumNumberOfCalls() {
		return minimumNumberOfCalls;
	}
	public void setMinimumNumberOfCalls(Integer minimumNumberOfCalls) {
		this.minimumNumberOfCalls = minimumNumberOfCalls;
	}
	
}
