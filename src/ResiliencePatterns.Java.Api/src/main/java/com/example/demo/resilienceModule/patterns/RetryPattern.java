package com.example.demo.resilienceModule.patterns;

import java.time.Duration;

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
	
	private long time;
	
	private long errorTime;

	public RetryPattern(Options params, Result result) {
		connector = new Connector(params.getUrlConfiguration(), params.getRequestConfiguration().getTimeout());
		this.createAndConfigRetry(params.getRetryConfiguration(), result);
	}

	@Override
	public boolean request(Result result, Options options) {
		time = errorTime = System.currentTimeMillis();
		result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
		if(Try.of(retryableSupplier).recover(throwable -> {
			//so se quebrar o maximo de vezes
			errorTime = System.currentTimeMillis() - errorTime;
			result.getResilienceModuleToExternalService().setTotalErrorTime( 
					result.getResilienceModuleToExternalService().getTotalErrorTime() + 
					errorTime);
			return false;
		}).get()) {
			//entra no começo
			System.out.println("foi aqui 2");
			time = System.currentTimeMillis() - time;
			result.getResilienceModuleToExternalService().setTotalSuccessTime(
					result.getResilienceModuleToExternalService().getTotalSuccessTime() + time);
			
			result.getResilienceModuleToExternalService().setSuccess(result.getResilienceModuleToExternalService().getSuccess() + 1);
			
			return true;
		} 
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
			//TODO quebrando
			System.out.println("foi aqui");
			time = System.currentTimeMillis() - time;
			result.getResilienceModuleToExternalService().setTotalSuccessTime(
					result.getResilienceModuleToExternalService().getTotalSuccessTime() + time);
			//result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
			//result.getResilienceModuleToExternalService().setSuccess(result.getResilienceModuleToExternalService().getSuccess() + 1);
		})
		.onError(event -> {
			//System.out.println("falhou o maximo de vezes");
			//result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
			//result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
			result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
		}).onRetry(event -> {
			
			errorTime = System.currentTimeMillis() - errorTime;
			result.getResilienceModuleToExternalService().setTotalErrorTime( 
					result.getResilienceModuleToExternalService().getTotalErrorTime() + 
					errorTime);
			
			//reinicia timestamps
			time = errorTime = System.currentTimeMillis();
			
			result.getRetryMetrics().setRetryCount(result.getRetryMetrics().getRetryCount() + 1);
			//System.out.println("falhou, vai retentar");
			result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
			result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
		});

		retryableSupplier = Retry.decorateCheckedSupplier(this.retry, connector::makeRequest);

	}

}
