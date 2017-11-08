""" Testing module of the PNet deep neural network (First MTCNN Layer) """

import tensorflow as tf
import Detectors.Mtcnn as mtcnn
import json
import CommonTest as common

def findFaces():
    with tf.Graph().as_default():
        session = tf.Session()
        
        with session.as_default():
            pnet = mtcnn.PNet()
            pnet.createNetwork()

            with open('./Tests/PNet.json') as dataFile:
                jsonWeights = json.load(dataFile)
            
            pnet.loadManualWeights(session, jsonWeights)

            
            img = common.getTestImageWHC('./Tests/Images/image_020_1.jpg', 615, 384)

            results = pnet.run(session, {'data': img}, ['prob1', 'conv4-2'])
            print(results)

findFaces()
