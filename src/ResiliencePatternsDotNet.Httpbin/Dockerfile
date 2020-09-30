FROM ubuntu:18.04

LABEL name="httpbin"
LABEL version="0.9.2"
LABEL description="A simple HTTP service."
LABEL org.kennethreitz.vendor="Kenneth Reitz"

RUN apt update -y && apt install python3-pip -y && apt install python-pip -y

RUN pip3 install --no-cache-dir gevent
RUN pip3 install --no-cache-dir gunicorn
RUN pip3 install --no-cache-dir httpbin
RUN pip install --no-cache-dir vaurien
RUN apt install supervisor -y

COPY ./assets /
CMD [ "/usr/bin/supervisord", "-c", "/etc/supervisor/supervisord.conf" ]

# vaurien http proxy port
EXPOSE 80

# httpin port
EXPOSE 9000

# vaurien api
EXPOSE 8080