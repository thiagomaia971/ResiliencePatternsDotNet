FROM ubuntu:18.04

LABEL name="httpbin"
LABEL version="0.9.2"
LABEL description="A simple HTTP service."
LABEL org.kennethreitz.vendor="Kenneth Reitz"

RUN apt update -y && apt install python3-pip -y

RUN pip3 install --no-cache-dir gevent
RUN pip3 install --no-cache-dir gunicorn
RUN pip3 install --no-cache-dir httpbin
RUN apt install supervisor -y

# httpin port
EXPOSE 80

COPY ./assets /
CMD [ "/usr/bin/supervisord", "-c", "/etc/supervisor/supervisord.conf" ]