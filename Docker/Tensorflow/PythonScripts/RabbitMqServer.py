""" Module that handles RabbitMq connecction and messages """

import json
import sys
import time
import argparse
import asyncio
import aioamqp
import Dispatchers as dispatchers

def get_message(message):
    """ Gets a RabbitMq biary messages and returs its deseerialized dictionary
        Keyword arguments:
        message -- RabbitMq binary message
    """
    json_message = message.decode('utf-8')
    return json.loads(json_message)

@asyncio.coroutine
def on_onet_request(channel, body, envelope, properties):
    """ Process the onet rabbitMq message
        Keyword arguments:
        channel -- RabbitMq channel where the message was received
        body -- Binary content of the message
        envelope -- Information associated with the message delivery
        properties -- Additional information associated with the message. Normally it contains
            the additional headers added by the client sender
    """
    message = get_message(body)
    response = dispatchers.onet(message)
    yield from channel.basic_publish(
        payload=json.dumps(response),
        exchange_name='',
        routing_key=properties.reply_to,
        properties={
            'correlation_id': properties.correlation_id,
        },
    )
    yield from channel.basic_client_ack(delivery_tag=envelope.delivery_tag)

@asyncio.coroutine
def on_pnet_request(channel, body, envelope, properties):
    """ Process the pnet rabbitMq message
        Keyword arguments:
        channel -- RabbitMq channel where the message was received
        body -- Binary content of the message
        envelope -- Information associated with the message delivery
        properties -- Additional information associated with the message. Normally it contains
            the additional headers added by the client sender
    """
    message = get_message(body)
    try:
        response = dispatchers.pnet(message)
    except Exception as ex:
        response = {'isSuccess': False, 'errorMessage': 'Unexpected error:' + str(ex)}
    yield from channel.basic_publish(
        payload=json.dumps(response),
        exchange_name='',
        routing_key=properties.reply_to,
        properties={
            'correlation_id': properties.correlation_id,
        },
    )
    yield from channel.basic_client_ack(delivery_tag=envelope.delivery_tag)

@asyncio.coroutine
def on_rnet_request(channel, body, envelope, properties):
    """ Process the rnet rabbitMq message
        Keyword arguments:
        channel -- RabbitMq channel where the message was received
        body -- Binary content of the message
        envelope -- Information associated with the message delivery
        properties -- Additional information associated with the message. Normally it contains
            the additional headers added by the client sender
    """
    message = get_message(body)
    response = dispatchers.rnet(message)
    yield from channel.basic_publish(
        payload=json.dumps(response),
        exchange_name='',
        routing_key=properties.reply_to,
        properties={
            'correlation_id': properties.correlation_id,
        },
    )
    yield from channel.basic_client_ack(delivery_tag=envelope.delivery_tag)

@asyncio.coroutine
def rpc_server(hostname):
    """ Starts up the rpc server and connect it to the RabbitMq service bus
        Keyword arguments:
        hostname -- Name of the host where RabbitMq is running
    """
    transport, protocol = yield from aioamqp.connect(host=hostname, port=5672)

    print('Connected to RabbitMQ, loading exchange and channels')

    channel = yield from protocol.channel()

    # yield from channel.queue_declare(queue_name='evaluate.onet')
    yield from channel.basic_qos(prefetch_count=1, prefetch_size=0, connection_global=False)
    yield from channel.basic_consume(on_onet_request, queue_name='evaluate.onet')
    yield from channel.basic_consume(on_pnet_request, queue_name='evaluate.pnet')
    yield from channel.basic_consume(on_rnet_request, queue_name='evaluate.rnet')
    print(" [x] Awaiting RPC requests")

def port_type(value):
    """ Tries to parse the input port, generating an error when it is invalid. Valid
        ports are between 0 - 65535
    Keyword arguments:
    value - Input port value as string
    """
    ivalue = int(value)
    if ivalue <= 0 or ivalue > 65535:
        raise argparse.ArgumentTypeError("%s is an invalid port number" % value)
    return ivalue

def parse_arguments():
    """ Parses the argument from the command line """
    parser = argparse.ArgumentParser(description='Run RabbitMQ server to process'
                                     + 'the different messages from the service bus.')
    parser.add_argument("servicebushost", help="Machine name of the service bus ")
    parser.add_argument("servicebusport", type=port_type, default=5672,
                        help="Machine port where AMPQ service is listening of the service bus ")
    return parser.parse_args()

def run_rpc_server():
    """ Run the Rpc server to listen messages comming from the RabbitMq queue """
    args = parse_arguments()
    print('Waiting to RabbitMQ to be online')
    time.sleep(1)
    print('Connecting to RabbitMQ on: ' + args.servicebushost)

    event_loop = asyncio.get_event_loop()
    event_loop.run_until_complete(rpc_server(args.servicebushost))
    event_loop.run_forever()

run_rpc_server()
