package com.example.demo.mapper.result;

public class ClientToModule {
	
	private Integer success;
	private Integer error;
	private Integer total;
	
	public ClientToModule() {
		this.success = 0;
		this.error = 0;
		this.total = 0;
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
	
}
