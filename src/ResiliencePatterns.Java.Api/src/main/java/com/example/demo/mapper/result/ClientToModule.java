package com.example.demo.mapper.result;

public class ClientToModule {
	
	private Integer success;
	private Integer error;
	private Integer total;
	private Long totalTime;
	private Float averageTimePerRequest;
	
	public ClientToModule() {
		this.success = 0;
		this.error = 0;
		this.total = 0;
		this.totalTime = 0L;
		this.averageTimePerRequest = 0F;
	}
	
	public Integer getSuccess() {
		return success;
	}
	public void setSuccess(Integer success) {
		this.success = success;
	}
	public Integer getError() {
		return error;
	}
	public void setError(Integer error) {
		this.error = error;
	}
	public Integer getTotal() {
		return total;
	}
	public void setTotal(Integer total) {
		this.total = total;
	}

	public Long getTotalTime() {
		return totalTime;
	}

	public void setTotalTime(Long totalTime) {
		this.totalTime = totalTime;
	}

	public Float getAverageTimePerRequest() {
		return averageTimePerRequest;
	}

	public void setAverageTimePerRequest(Float averageTimePerRequest) {
		this.averageTimePerRequest = averageTimePerRequest;
	}
	
}
