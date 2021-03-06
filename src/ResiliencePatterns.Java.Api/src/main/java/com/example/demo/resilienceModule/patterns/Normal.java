package com.example.demo.resilienceModule.patterns;

import com.example.demo.mapper.Options;
import com.example.demo.mapper.result.Result;
import com.example.demo.resilienceModule.Connector;

public class Normal implements Pattern {
	
	private Connector connector;
	
	public Normal(Options params, Result result) {
		connector = new Connector(params.getUrlConfiguration(), params.getRequestConfiguration().getTimeout());
		
	}

	@Override
	public boolean request(Result result, Options options) {
		long time, errorTime = 0;
		result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() +1);
		try {
			time = errorTime = System.currentTimeMillis();
			connector.makeRequest();
			time = System.currentTimeMillis() - time;
			result.getResilienceModuleToExternalService().setTotalSuccessTime(
					result.getResilienceModuleToExternalService().getTotalSuccessTime() + time);
			result.getResilienceModuleToExternalService().setSuccess(result.getResilienceModuleToExternalService().getSuccess() + 1);
		}catch(Exception ex) {
			long eTime = System.currentTimeMillis() - errorTime;
			result.getResilienceModuleToExternalService().setTotalErrorTime(
				result.getResilienceModuleToExternalService().getTotalErrorTime() +
				eTime);
			result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
			return false;
		}
		return true;
	}

}
