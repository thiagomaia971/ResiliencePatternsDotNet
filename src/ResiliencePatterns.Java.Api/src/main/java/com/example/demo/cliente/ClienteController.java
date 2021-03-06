package com.example.demo.cliente;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;

import com.example.demo.mapper.Options;
import com.example.demo.mapper.result.Result;
import com.example.demo.resilienceModule.patterns.CircuitBreakerPattern;
import com.example.demo.resilienceModule.patterns.Normal;
import com.example.demo.resilienceModule.patterns.Pattern;
import com.example.demo.resilienceModule.patterns.RetryPattern;

import io.github.resilience4j.common.retry.configuration.RetryConfigCustomizer;

@Controller
public class ClienteController {
	
	private CircuitBreakerPattern cb;
	
	private Normal normal;
	
	private RetryPattern retry;
	
	@PostMapping("/execute")
	public ResponseEntity<?> normalRequests(@RequestBody(required = false) Options options) {
		Result result = new Result();
		
		Pattern pattern = this.getPattern(options, result);
		
		long time = System.currentTimeMillis();
		for (int i = 0; stopCondition(i, options, result) ; i++) {
			if(pattern.request(result, options)) {
				result.getClientToModule().setSuccess(result.getClientToModule().getSuccess() + 1);
			} else {
				result.getClientToModule().setError(result.getClientToModule().getError() + 1);
			}
		}
		time = System.currentTimeMillis() - time;
		result.getClientToModule().setTotalTime(time);
		
		adjustParamsForCircuitBreaker(options, result);
		
		result.getClientToModule().setTotal(result.getClientToModule().getSuccess() + result.getClientToModule().getError());
		result.getClientToModule().setAverageTimePerRequest((float) result.getClientToModule().getTotalTime() / result.getClientToModule().getTotal() );
		
		
		
		if(result.getResilienceModuleToExternalService().getSuccess() > 0) {
			result.getResilienceModuleToExternalService().setAverageSuccessTimePerRequest( 
					(float) result.getResilienceModuleToExternalService().getTotalSuccessTime() / 
					result.getResilienceModuleToExternalService().getSuccess() );
		}
		
		return new ResponseEntity<>(result, HttpStatus.OK);
	}
	
	
	private Pattern getPattern(Options options, Result result) {
		String patternKey = options.getRunPolicy();
		if(patternKey.equalsIgnoreCase(Util.NORMAL_PATTERN_KEY)) {
			normal = new Normal(options, result);
			return this.normal;
		} else if(patternKey.equalsIgnoreCase(Util.CIRCUIT_BREAKER_PATTERN_KEY)) {
			cb = new CircuitBreakerPattern(options, result);
			return cb;
		} else if(patternKey.equalsIgnoreCase(Util.RETRY_PATTERN_KEY)) {
			retry = new RetryPattern(options, result);
			return retry;
		}
		return normal;
	}
	
	private boolean stopCondition(int i, Options options, Result result) {
		if( options.getRunPolicy().equalsIgnoreCase(Util.CIRCUIT_BREAKER_PATTERN_KEY) && options.getRequestConfiguration().getMaxRequests() == null) {
			return result.getResilienceModuleToExternalService().getSuccess() < options.getRequestConfiguration().getSuccessRequests();
		} else {
			return i < options.getRequestConfiguration().getMaxRequests() &&
					result.getClientToModule().getSuccess() < options.getRequestConfiguration().getSuccessRequests();
		}
	}
	
	private void adjustParamsForCircuitBreaker(Options options, Result result) {
		if(options.getRunPolicy().equalsIgnoreCase(Util.CIRCUIT_BREAKER_PATTERN_KEY))
			result.getClientToModule().setError(result.getResilienceModuleToExternalService().getError());
	}


}
