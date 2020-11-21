package com.example.demo.resilienceModule.patterns;

import java.time.Duration;

import org.springframework.http.ResponseEntity;

import com.example.demo.mapper.Options;
import com.example.demo.mapper.RetryConfiguration;
import com.example.demo.mapper.result.Result;
import com.example.demo.resilienceModule.Connector;

import io.github.resilience4j.core.IntervalFunction;
import io.github.resilience4j.retry.Retry;
import io.github.resilience4j.retry.RetryConfig;
import io.github.resilience4j.retry.RetryRegistry;
import io.vavr.CheckedFunction0;
import io.vavr.control.Try;

public class RetryPattern implements Pattern {

	private Connector connector;

	private Retry retry;

	private CheckedFunction0<Boolean> retryableSupplier;
	
	private long errorTime, accumlatedErrorTime, successTime, accumulatedSuccessTime, inactiveTime, accumulatedInactiveTime;
	

	

	public RetryPattern(Options params, Result result) {
		connector = new Connector(params.getUrlConfiguration(), params.getRequestConfiguration().getTimeout());
		this.createAndConfigRetry(params.getRetryConfiguration(), result);
	}

	@Override
	public boolean request(Result result, Options options) {
		successTime = errorTime = System.currentTimeMillis();
		result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
		if(Try.of(retryableSupplier).recover(throwable -> false).get()) {
			result.getResilienceModuleToExternalService().setSuccess(result.getResilienceModuleToExternalService().getSuccess() + 1);
			fillResultMetrics(result);
			return true;
		} 
		fillResultMetrics(result);
		return false;
	}

	public void createAndConfigRetry(RetryConfiguration params, Result result) {
		RetryConfig config;
		
		params.setCount(params.getCount() + 1);
		
		if(params.getSleepDurationType().equalsIgnoreCase("EXPONENTIAL_BACKOFF")) {
			config = RetryConfig.custom()
					.maxAttempts(params.getCount())
					.waitDuration(Duration.ofMillis(params.getSleepDuration()))
					.intervalFunction(IntervalFunction.ofExponentialBackoff(params.getSleepDuration()))
					.build();
		} else {
			config = RetryConfig.custom()
					.maxAttempts(params.getCount())
					.waitDuration(Duration.ofMillis(params.getSleepDuration()))
					.intervalFunction(IntervalFunction.ofDefaults())
					.build();
		}

		RetryRegistry registry = RetryRegistry.of(config);

		this.retry = registry.retry("retry1");

		retry.getEventPublisher()
		.onSuccess(event -> {

		})
		.onError(event -> {
			result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
		}).onRetry(event -> {
			inactiveTime = System.currentTimeMillis();
			accumlatedErrorTime += (System.currentTimeMillis() - errorTime);
			
			result.getRetryMetrics().setRetryCount(result.getRetryMetrics().getRetryCount() + 1);
			result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
			result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
		});

		retryableSupplier = Retry.decorateCheckedSupplier(this.retry, this::prepareRequest);
		
	}
	
	
	private Boolean prepareRequest() {
		//reinicia timestamps
		successTime  = errorTime = System.currentTimeMillis();
		if(inactiveTime != 0 ) { //veio de um retry
			accumulatedInactiveTime += (System.currentTimeMillis() - inactiveTime);
			inactiveTime = 0;
		}
		connector.makeRequest();
		accumulatedSuccessTime += (System.currentTimeMillis() - successTime);
		return true;
	}
	
	private void fillResultMetrics(Result result) {
		result.getRetryMetrics().setTotalTimeout(accumulatedInactiveTime);
		result.getResilienceModuleToExternalService().setTotalSuccessTime(accumulatedSuccessTime);
		result.getResilienceModuleToExternalService().setTotalErrorTime(accumlatedErrorTime);
	}
	
	

}
