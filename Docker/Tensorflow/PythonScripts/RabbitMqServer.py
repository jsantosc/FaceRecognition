import argparse
import asyncio
import aioamqp
import json
import time
import Dispatchers as dispatchers

def getMessage(message):
    json = message.decode('utf-8')
    return json.loads(json)

@asyncio.coroutine
def onONetRequest(channel, body, envelope, properties):
    message = getMessage(body)
    respose = dispatchers.onet(message)
    yield from channel.basic_publish(
        payload= json.dumps(response),
        exchange_name='',
        routing_key=properties.reply_to,
        properties={
            'correlation_id': properties.correlation_id,
        },
    )
    yield from channel.basic_client_ack(delivery_tag=envelope.delivery_tag)

@asyncio.coroutine
def onPNetRequest(channel, body, envelope, properties):
    message = getMessage(body)
    respose = dispatchers.pnet(message)
    yield from channel.basic_publish(
        payload = json.dumps(response),
        exchange_name='',
        routing_key=properties.reply_to,
        properties={
            'correlation_id': properties.correlation_id,
        },
    )
    yield from channel.basic_client_ack(delivery_tag=envelope.delivery_tag)

@asyncio.coroutine
def onRNetRequest(channel, body, envelope, properties):
    message = getMessage(body)
    respose = dispatchers.rnet(message)
    yield from channel.basic_publish(
        payload = json.dumps(response),
        exchange_name='',
        routing_key=properties.reply_to,
        properties={
            'correlation_id': properties.correlation_id,
        },
    )
    yield from channel.basic_client_ack(delivery_tag=envelope.delivery_tag)
    

@asyncio.coroutine
def rpc_server(hostname):

    transport, protocol = yield from aioamqp.connect(host=hostname, port=5672)

    print('Connected to RabbitMQ, loading exchange and channels')

    channel = yield from protocol.channel()

    # yield from channel.queue_declare(queue_name='evaluate.onet')
    yield from channel.basic_qos(prefetch_count=1, prefetch_size=0, connection_global=False)
    yield from channel.basic_consume(onONetRequest, queue_name='evaluate.onet')
    yield from channel.basic_consume(onPNetRequest, queue_name='evaluate.pnet')
    yield from channel.basic_consume(onRNetRequest, queue_name='evaluate.rnet')
    print(" [x] Awaiting RPC requests")

def port_type(value):
    ivalue = int(value)
    if ivalue <= 0 or ivalue > 65535:
        raise argparse.ArgumentTypeError("%s is an invalid port number" % value)
    return ivalue

parser = argparse.ArgumentParser(description='Run RabbitMQ server to process the different messages from the service bus.')
parser.add_argument("servicebushost", help="Machine name of the service bus ")
parser.add_argument("servicebusport", type=port_type, default=5672, help="Machine port where AMPQ service is listening of the service bus ")
args = parser.parse_args()

print('Waiting to RabbitMQ to be online')
time.sleep(1)
print('Connecting to RabbitMQ on: ' + args.servicebushost)

event_loop = asyncio.get_event_loop()
event_loop.run_until_complete(rpc_server(args.servicebushost))
event_loop.run_forever()