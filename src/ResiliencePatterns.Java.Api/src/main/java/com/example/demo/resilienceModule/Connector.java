package com.example.demo.resilienceModule;

import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;
import org.springframework.web.client.RestTemplate;

import com.example.demo.mapper.UrlConfiguration;


public class Connector {

	private RestTemplate restTemplate;
	private HttpEntity<String> entity = new HttpEntity<String>(new HttpHeaders());
	
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
	
	public ResponseEntity<?> makeRequest() {
		ResponseEntity<?> res = restTemplate.exchange(url, HttpMethod.GET, entity, String.class); 
		return res;
	}
}
