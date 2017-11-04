""" Testing module of the PNet deep neural network (First MTCNN Layer) """

import tensorflow as tf
import Detectors.Mtcnn as mtcnn
import json

def findFaces():
    with tf.Graph().as_default():
        session = tf.Session()
        
        with session.as_default():
            pnet = mtcnn.PNet()
            pnet.createNetwork()

            with open('./Tests/PNet.json') as dataFile:
                jsonWeights = json.load(dataFile)
            
            pnet.loadManualWeights(session, jsonWeights)

findFaces()
