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


#CMD ["gunicorn", "-b", "0.0.0.0:80", "httpbin:app", "-k", "gevent"]

# FROM python:2-alpine

# RUN pip install --upgrade pip

# # install vaurien
# RUN apk add --no-cache \
#         gcc \
#     musl-dev
# RUN pip install vaurien

# # install httpbin
# RUN apk add --no-cache \
#         libffi-dev \
#         build-base
# RUN pip install httpbin gunicorn

# # install supervisord
# RUN apk add --no-cache \
#     supervisor

# COPY ./assets /
# CMD [ "/usr/bin/supervisord", "-c", "/etc/supervisor/supervisord.conf" ]

# # vaurien http proxy port
# EXPOSE 80

# # httpin port
# EXPOSE 9000

