FROM ubuntu:18.04

LABEL name="httpbin"
LABEL version="0.9.2"
LABEL description="A simple HTTP service."
LABEL org.kennethreitz.vendor="Kenneth Reitz"

RUN apt update -y && apt install && apt install python-pip -y

RUN pip install --no-cache-dir vaurien
RUN apt install supervisor -y

# vaurien http proxy port
EXPOSE 80

COPY ./assets /
CMD [ "/usr/bin/supervisord", "-c", "/etc/supervisor/supervisord.conf" ]