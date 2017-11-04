""" Input layer. Holds the nodes with the values for the image """

import tensorflow as tf
import Networks.Tensorflow.Layer as l

class InputLayer(l.Layer):
    ''' Input layer to feed the neural network '''
    def __init__(self, layerName, size):
        super(InputLayer, self).__init__(layerName)
        self.size = size
        self.inputTensor = tf.placeholder(tf.float32, (None,None,None,3), layerName)
        self.setOutput(self.inputTensor)
        self.setType('Input')
        