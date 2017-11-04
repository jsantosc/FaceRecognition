""" Module that contains all the softmax layer implementation in tensorflow. It supports
multidimensional inputs, as described in the issue https://github.com/tensorflow/tensorflow/issues/210 """

import tensorflow as tf
import Networks.Tensorflow.Layer as l

class SoftmaxLayer(l.Layer):
    
    def __init__(self, layerName, inputNode, axis):
        super(SoftmaxLayer, self).__init__(layerName)

        self.axis = axis
        
        with tf.variable_scope(layerName):
            self.max_axis = tf.reduce_max(inputNode, axis, keep_dims=True)
            self.target_exp = tf.exp(inputNode-self.max_axis)
            self.normalize = tf.reduce_sum(self.target_exp, axis, keep_dims=True)
            self.softmax = self.target_exp / self.normalize

        self.setOutput(self.softmax)

        self.setType('Softmax')
        
