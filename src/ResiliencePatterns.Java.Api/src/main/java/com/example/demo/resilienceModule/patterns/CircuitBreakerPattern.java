package com.example.demo.resilienceModule.patterns;

import java.time.Duration;
import java.util.function.Supplier;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;

import com.example.demo.mapper.CircuitBreakerConfiguration;
import com.example.demo.mapper.Options;
import com.example.demo.mapper.result.Result;
import com.example.demo.resilienceModule.Connector;

import io.github.resilience4j.circuitbreaker.CircuitBreaker;
import io.github.resilience4j.circuitbreaker.CircuitBreaker.State;
import io.github.resilience4j.circuitbreaker.CircuitBreakerConfig;
import io.github.resilience4j.circuitbreaker.CircuitBreakerRegistry;
import io.vavr.control.Try;


public class CircuitBreakerPattern implements Pattern {
	
	private Connector connector;
	
	private CircuitBreaker circuitBreaker;
	
	private Supplier<Boolean> decoratedSupplier;
	
	private long totalOfBreak;
	
	private long errorTime, accumlatedErrorTime, successTime, accumulatedSuccessTime, inactiveTime, accumulatedInactiveTime;
	
	public CircuitBreakerPattern(Options params, Result result) {
		connector = new Connector(params.getUrlConfiguration(), params.getRequestConfiguration().getTimeout());
		this.createAndConfigCircuitBreaker(params.getCircuitBreakerConfiguration(), result);
	}

	public boolean request(Result result, Options options) {
		long time,errorTime;
		time = errorTime = System.currentTimeMillis();
		boolean aux = Try.ofSupplier(decoratedSupplier).recover(throwable -> {
			//long eTime = System.currentTimeMillis() - errorTime;
			//result.getResilienceModuleToExternalService().setTotalErrorTime(
			//	result.getResilienceModuleToExternalService().getTotalErrorTime() +
			//	eTime);
			return false;}).get();
		fillResultMetrics(result);
		result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
		if(aux) {
			time = System.currentTimeMillis() - time;
			result.getResilienceModuleToExternalService().setTotalSuccessTime(
					result.getResilienceModuleToExternalService().getTotalSuccessTime() + time);
			return true;
		}
		return false;
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
			accumlatedErrorTime += (System.currentTimeMillis() - errorTime);
			result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
		})
		.onStateTransition(event -> {
			if( event.getStateTransition().getToState().toString().equals(State.OPEN.toString()) ) {
				inactiveTime = System.currentTimeMillis();
				result.getCircuitBreakerMetrics().setBreakCount( result.getCircuitBreakerMetrics().getBreakCount() + 1 );
			}
			
		});
		
		decoratedSupplier = CircuitBreaker
	            .decorateSupplier(circuitBreaker, this::prepareRequest);
		
	}
	
	
	private Boolean prepareRequest() {
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
		result.getCircuitBreakerMetrics().setTotalOfBreak(accumulatedInactiveTime);
		result.getResilienceModuleToExternalService().setTotalSuccessTime(accumulatedSuccessTime);
		result.getResilienceModuleToExternalService().setTotalErrorTime(accumlatedErrorTime);
		
	}

	
}
