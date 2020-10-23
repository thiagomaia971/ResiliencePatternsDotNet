package com.example.demo.mapper;

public class RetryConfiguration {
	private Integer count = 3; //maxAttempts
	private Integer sleepDuration = 500; //waitDuration [ms]
	private String sleepDurationType = "FIXED"; //FIXED / EXPONENTIAL_BACKOFF
	
	public Integer getCount() {
		return count;
	}
	public void setCount(Integer count) {
		this.count = count;
	}
	public Integer getSleepDuration() {
		return sleepDuration;
	}
	public void setSleepDuration(Integer sleepDuration) {
		this.sleepDuration = sleepDuration;
	}
	public String getSleepDurationType() {
		return sleepDurationType;
	}
	public void setSleepDurationType(String sleepDurationType) {
		this.sleepDurationType = sleepDurationType;
	}
	
	
}
