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
		try {
			result.getResilienceModuleToExternalService().setTotal(result.getResilienceModuleToExternalService().getTotal() + 1);
			long time = System.currentTimeMillis();
			connector.makeRequest();
			result.getResilienceModuleToExternalService().setTotalSuccessTime(
					result.getResilienceModuleToExternalService().getTotalSuccessTime() + time);
			result.getResilienceModuleToExternalService().setSuccess(result.getResilienceModuleToExternalService().getSuccess() + 1);
		}catch(Exception ex) {
			result.getResilienceModuleToExternalService().setError(result.getResilienceModuleToExternalService().getError() + 1);
			return false;
		}
		return true;
	}

}
