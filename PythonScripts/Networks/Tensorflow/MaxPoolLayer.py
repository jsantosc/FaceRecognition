""" Module with the class definitions to perform a Max Pooling operation in a tensorflow deep neural network """

import tensorflow as tf
import Networks.Tensorflow.Layer as l
import Networks.Tensorflow.Common as common

class MaxPoolLayer(l.Layer):
    def __init__(self, layerName, inputNode, kernelHeight, kernelWidth, strideHeight, strideWidth, padding='SAME'):
        super(MaxPoolLayer, self).__init__(layerName)

        common.validatePadding(padding)

        self.inputNode = inputNode
        self.kernelSize = { "width": kernelWidth, "height": kernelHeight }
        self.strideSize = { "width": strideWidth, "height": strideHeight }
        self.padding = padding

        self.maxPoolingOp = tf.nn.max_pool(inputNode,
                              ksize=[1, kernelHeight, kernelWidth, 1],
                              strides=[1, strideHeight, strideWidth, 1],
                              padding=padding,
                              name=layerName)
        self.setOutput(self.maxPoolingOp);
        