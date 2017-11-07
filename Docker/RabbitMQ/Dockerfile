FROM rabbitmq:3.6.12-management-alpine
MAINTAINER <jsantosc@gmail.com>

ARG envname

RUN echo "Using environment $envname"

COPY ${envname}/rabbitmq.config /etc/rabbitmq/
COPY ${envname}/definitions.json /etc/rabbitmq/

CMD ["rabbitmq-server"]
