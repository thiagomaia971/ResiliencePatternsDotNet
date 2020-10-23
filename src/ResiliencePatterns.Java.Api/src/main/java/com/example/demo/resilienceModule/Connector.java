package com.example.demo.resilienceModule;

import org.springframework.web.client.RestTemplate;

import com.example.demo.mapper.UrlConfiguration;


public class Connector {

	private RestTemplate restTemplate;
	
	private String url;
	private String method;
	
	public Connector(UrlConfiguration urlConfiguration, Integer timeout) {
		//if(timeout != null) {
		//	HttpComponentsClientHttpRequestFactory httpRequestFactory = new HttpComponentsClientHttpRequestFactory();
		//	httpRequestFactory.setReadTimeout(timeout);
			//httpRequestFactory.setConnectionRequestTimeout(timeout);
	        //httpRequestFactory.setConnectTimeout(timeout);
	        //https://stackoverflow.com/questions/27749939/httpclient-4-3-5-connectionrequesttimeout-vs-connecttimeout-for-httpconnectionpa
	   //     restTemplate = new RestTemplate(httpRequestFactory);
		//} else {
			restTemplate = new RestTemplate();
		//}
		this.url = urlConfiguration.getBaseUrl() + urlConfiguration.getAction();
		this.method = urlConfiguration.getMethod();
	}
	
	public Boolean makeRequest() {
		if(method.equalsIgnoreCase("POST")) {
			restTemplate.postForEntity(url, null, String.class);
		} else {
			restTemplate.getForObject(url, String.class);
		}
		return true;
	}
}
