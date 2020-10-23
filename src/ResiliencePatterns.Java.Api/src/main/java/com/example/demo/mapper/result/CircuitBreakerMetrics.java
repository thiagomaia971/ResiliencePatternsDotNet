package com.example.demo.mapper.result;

public class CircuitBreakerMetrics {
	
	private Integer breakCount;
	private Integer resetStatCount;
	private Long totalOfBreak;
	
	public CircuitBreakerMetrics() {
		breakCount = 0;
		resetStatCount = 0;
		totalOfBreak = 0L;
		
	}
			
	public Integer getBreakCount() {
		return breakCount;
	}
	public void setBreakCount(Integer breakCount) {
		this.breakCount = breakCount;
	}
	public Integer getResetStatCount() {
		return resetStatCount;
	}
	public void setResetStatCount(Integer resetStatCount) {
		this.resetStatCount = resetStatCount;
	}
	public Long getTotalOfBreak() {
		return totalOfBreak;
	}
	public void setTotalOfBreak(Long totalOfBreak) {
		this.totalOfBreak = totalOfBreak;
	}
	
	
	
	
}
