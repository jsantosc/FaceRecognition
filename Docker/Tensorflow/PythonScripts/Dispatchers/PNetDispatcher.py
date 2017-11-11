""" Module for pnet tensorflow network messages dispatching """

import json
import tensorflow as tf
import numpy as np
import Detectors.Mtcnn as mtcnn

def dispatch(message):
    """ Process a pnet rabbitMq message, returning the execution results of the PNet network
        Keyword arguments:
        message -- RabbotMq PNet message
    """
    with tf.Graph().as_default():
        session = tf.Session()
        pnet = mtcnn.PNet()
        pnet.createNetwork()

        if message['loadWeightsMode'] == 0:
            with open(message['weightsFilePath']) as data_file:
                json_weights = json.load(data_file)

        pnet.loadManualWeights(session, json_weights)
        input_values = {}
        for input_layer_value in message['inputLayerValues']:
            img_with_batch = np.expand_dims(input_layer_value['value'], 0)
            input_values = dict(input_values, **{input_layer_value['name']: img_with_batch})

        results = pnet.run(session, input_values, message['outputLayerNames'])

    serializable_results = []
    for item in results:
        serializable_results.append({'name':item['name'], 'value':item['value'].tolist()})
    return {'isSuccess': True, 'errorMessage': '', 'outputValues': serializable_results}
