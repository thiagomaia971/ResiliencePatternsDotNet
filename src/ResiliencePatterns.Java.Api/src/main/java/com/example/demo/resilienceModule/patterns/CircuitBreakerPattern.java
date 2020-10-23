package com.example.demo.resilienceModule.patterns;

import java.time.Duration;
import java.util.function.Supplier;

import com.example.demo.mapper.CircuitBreakerConfiguration;
import com.example.demo.mapper.Options;
import com.example.demo.mapper.result.Result;
import com.example.demo.resilienceModule.Connector;

import io.github.resilience4j.circuitbreaker.CircuitBreaker;
import io.github.resilience4j.circuitbreaker.CircuitBreakerConfig;
import io.github.resilience4j.circuitbreaker.CircuitBreakerRegistry;
import io.vavr.control.Try;


public class CircuitBreakerPattern implements Pattern {
	
	private Connector connector;
	
	private CircuitBreaker circuitBreaker;
	
	private Supplier<Boolean> decoratedSupplier;
	
	public CircuitBreakerPattern(Options params, Result result) {
		connector = new Connector(params.getUrlConfiguration(), params.getRequestConfiguration().getTimeout());
		this.createAndConfigCircuitBreaker(params.getCircuitBreakerConfiguration(), result);
	}

	public boolean request(Result result, Options options) {
		long time = System.currentTimeMillis();
		result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
		boolean aux = Try.ofSupplier(decoratedSupplier).recover(throwable -> false).get();
		result.getResilienceModuleToExternalService().setTotalSuccessTime(
				result.getResilienceModuleToExternalService().getTotalSuccessTime() + time);
		return aux;
	}
	
	public void createAndConfigCircuitBreaker(CircuitBreakerConfiguration params, Result result) {
		CircuitBreakerConfig circuitBreakerConfig = CircuitBreakerConfig.custom()
			    .failureRateThreshold(params.getFailureThreshold())
			    .slidingWindowSize(params.getSlidingWindowSize())
			    .minimumNumberOfCalls(params.getMinimumNumberOfCalls())
			    .waitDurationInOpenState(Duration.ofMillis(params.getDurationOfBreaking()))
			    .permittedNumberOfCallsInHalfOpenState(params.getPermittedNumberOfCallsInHalfOpenState())
			    .slowCallRateThreshold(params.getSlowCallRateThreshold())
			    .slowCallDurationThreshold(Duration.ofMillis(params.getSlowCallDurationThreshold()))
			    .build();
		
		
		CircuitBreakerRegistry circuitBreakerRegistry =
				  CircuitBreakerRegistry.of(circuitBreakerConfig);
		
		this.circuitBreaker = circuitBreakerRegistry.circuitBreaker("cb1");
		//this.circuitBreaker = CircuitBreaker.ofDefaults("testName");
		
		circuitBreaker.getEventPublisher()
		.onSuccess(event -> {
			result.getResilienceModuleToExternalService().setSuccess(result.getResilienceModuleToExternalService().getSuccess() + 1);
		})
		.onError(event -> {
			result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
		})
		.onStateTransition(event -> {
			result.getCircuitBreakerMetrics().setBreakCount( result.getCircuitBreakerMetrics().getBreakCount() + 1 );
			System.out.println("mudou estado cb");
		});
		
		decoratedSupplier = CircuitBreaker
	            .decorateSupplier(circuitBreaker, connector::makeRequest);
		
	}

	
}
