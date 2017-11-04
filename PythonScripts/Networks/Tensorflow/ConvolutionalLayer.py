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
        self.biased = biased
        self.hasRelu = addRelu
        self.reluType = reluType

        self.setType('Convolutional')
        
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
    
    def loadManualWeights(self, session, weights):
        super(ConvolutionalLayer, self).loadManualWeights(session, weights)
        
        if weights['layerType'] != 'Convolutional':
            raise 'Invalid layer type ' + weights['layerType'] + '. Expected convolutional'
        if 'kernelWeights' not in weights:
            raise 'Kernel weights not defined. Expected property kernelWeights'
        if self.biased is True and 'biases' not in weights:
            raise 'Convolutional operation use biases. Expected property biases'
        if self.hasRelu is True and self.reluType == "PRELU" and 'preluAlphas' not in weights:
            raise 'Convolutional layer is implemented with a PRelu layer. Expected property preluAlphas'

        session.run(self.kernel.assign(weights['kernelWeights']))
        if self.biased is True:
            session.run(self.biases.assign(weights['biases']))
        if self.hasRelu is True and self.reluType == "PRELU":
            session.run(self.preluAlphas.assign(weights['preluAlphas']))

