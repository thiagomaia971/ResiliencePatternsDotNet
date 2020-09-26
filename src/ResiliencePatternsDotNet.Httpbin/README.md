Vaurien httpbin
===============

[Httpbin](https://github.com/requests/httpbin) served through [Vaurien chaos HTTP proxy](http://vaurien.readthedocs.io).


Intent
------

In order to test a monitoring software, I needed a HTTP service that randomly fails.


Configuration
-------------

Default Vaurien behavior is `1:blackout,5:delay,5:error,1:hang`. Hence: 
- 1% blackout
- 5% 70s delay
- 5% random 5xx HTTP error
- 1% hang

This can be change by mounting a file to `/etc/vaurien.ini` in the docker container. Example config file: [vaurien.ini](assets/etc/vaurien.ini)


Usage
-----

Start the Docker container: `docker run -d -p 8080:80 tomdesinto/vaurien-httpbin`

