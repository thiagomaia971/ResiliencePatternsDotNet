package com.example.demo.resilienceModule.patterns;

import com.example.demo.mapper.Options;
import com.example.demo.mapper.result.Result;

public interface Pattern {
	boolean request(Result result, Options options);
}
