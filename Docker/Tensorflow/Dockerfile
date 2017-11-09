FROM tensorflow/tensorflow:1.4.0-py3

LABEL maintainer="jsantosc@gmail.com"

ENV SERVICEBUSHOST servicebus
ENV SERVICEBUSPORT 5672

WORKDIR /facerec

COPY PythonScripts/ .

RUN pip3 install -r requirements.txt
