""" Module that creates a new convolutional layer """

import tensorflow as tf
import Networks.Tensorflow.Layer as l
import Networks.Tensorflow.Common as common

class ConvolutionalLayer(l.Layer):
    def __init__(self,
                network,
                layerName,
                inputNode,
                kernelWidth,
                kernelHeight,
                kernelChannels,
                strideHeight,
                strideWidth,
                padding='SAME',
                grouping=1,
                biased=True,
                addRelu=True,
                reluType='RELU'):
        super(ConvolutionalLayer, self).__init__(layerName)

        self.network = network
        self.inputNode = inputNode
        self.kernelSize = { "width": kernelWidth, "height": kernelHeight, "channels": kernelChannels }
        self.strides = { "width": strideWidth, "height": strideHeight }
        self.padding = padding
        self.grouping = grouping
        self.hasRelu = addRelu
        self.reluType = reluType
        
        common.validatePadding(padding) 

        inputChannels = int(inputNode.get_shape()[-1])
        assert inputChannels % grouping == 0
        assert kernelChannels % grouping == 0
        
        with tf.variable_scope(layerName) as scope:
            kernelShape = [kernelHeight, kernelWidth, inputChannels // grouping, kernelChannels]
            self.kernel = tf.get_variable('weights', kernelShape, trainable=network.isTrainable)
            self.convolution = tf.nn.conv2d(self.inputNode, self.kernel, [1, strideHeight, strideWidth, 1], padding=padding)
            self.setOutput(self.convolution)
            
            if biased:
                self.biases = tf.get_variable('biases', [kernelChannels], trainable=network.isTrainable)
                self.convolutionBiased = tf.nn.bias_add(self.convolution, self.biases)
                self.setOutput(self.convolutionBiased)
            if addRelu:
                common.validateReluType(reluType)
                if reluType == "RELU":
                    self.reluOutput = tf.nn.relu(self.getOutput())
                    self.setOutput(self.reluOutput)
                elif reluType == "PRELU":
 

                    channel_shared = False
                    if channel_shared:
                        w_shape = (1,)
                    else:
                        w_shape = (kernelChannels)
                    self.preluAlphas = tf.get_variable('preluAlphas', w_shape, trainable=network.isTrainable, initializer=tf.zeros_initializer)
                    self.reluOutput = tf.nn.relu(self.getOutput()) + tf.multiply(self.preluAlphas, (self.getOutput() - tf.abs(self.getOutput()))) * 0.5
                    self.setOutput(self.reluOutput)
