package com.example.demo.mapper.result;

public class ResilienceModuleToExternalService {

	private Integer success;
	private Integer error;
	private Integer total;
	private Long totalSuccessTime;
	
	public ResilienceModuleToExternalService() {
		this.success = 0;
		this.error = 0;
		this.total = 0;
		this.totalSuccessTime = 0L;
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

	public Long getTotalSuccessTime() {
		return totalSuccessTime;
	}

	public void setTotalSuccessTime(Long totalSuccessTime) {
		this.totalSuccessTime = totalSuccessTime;
	}
	
	
}
