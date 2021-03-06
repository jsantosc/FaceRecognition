""" Module containing basic tensorflow networks deep neural network """

import tensorflow as tf
import Networks.Tensorflow.InputLayer as il
import Networks.Tensorflow.ConvolutionalLayer as cl
import Networks.Tensorflow.MaxPoolLayer as mpl
import Networks.Tensorflow.SoftmaxLayer as sml

class Network(object):
    ''' Represents a new Tensorlflow deep neural network '''

    def __init__(self, name, trainable=True):
        ''' Constructs a new tensorflow neural network '''
        self.name = name
        self.isTrainable = trainable
        self.layers = {}
        self.inputLayers = {}
        self.currentOutputNode = None

    def addInputLayer(self, layerName, size):
        with tf.variable_scope(self.name):
            self.inputLayers = dict(self.inputLayers, **{ layerName: il.InputLayer(layerName, size) })

    def feed(self, layerName):
        if layerName in self.layers:
            self.currentOutputNode = self.layers[layerName]
            return self
        elif layerName in self.inputLayers:
            self.currentOutputNode = self.inputLayers[layerName]
            return self
        raise 'You must feed the network from a valid layer name'
    
    def addConvolutionalLayer(self,
                              layerName,
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
        if self.currentOutputNode == None:
            raise 'You must feed the network and choose the previous node'

        with tf.variable_scope(self.name):
            convLayer = cl.ConvolutionalLayer(self, layerName, self.currentOutputNode.getOutput(), kernelWidth, kernelHeight, kernelChannels,
                            strideHeight, strideWidth, padding, grouping, biased, addRelu, reluType)
            self.layers = dict(self.layers, **{ layerName: convLayer })
            self.currentOutputNode = convLayer
            return self
        
    def addMaxPooling(self, layerName, kernelHeight, kernelWidth, strideHeight, strideWidth, padding='SAME'):
        if self.currentOutputNode == None:
            raise 'You must feed the network and choose the previous node'

        with tf.variable_scope(self.name):
            maxPoolLayer = mpl.MaxPoolLayer(layerName, self.currentOutputNode.getOutput(), kernelHeight, kernelWidth, strideHeight, strideWidth, padding)
            self.layers = dict(self.layers, **{ layerName: maxPoolLayer })
            self.currentOutputNode = maxPoolLayer
            return self

    def addSoftmax(self, layerName, axis):
        if self.currentOutputNode == None:
            raise 'You must feed the network and choose the previous node'

        with tf.variable_scope(self.name):
            softmax = sml.SoftmaxLayer(layerName, self.currentOutputNode.getOutput(), axis)
            self.layers = dict(self.layers, **{ layerName: softmax })
            self.currentOutputNode = softmax
            return self

    def run(self, session, input, outputLayerNames):
        inputDict = {}
        for inputName, inputLayer in self.inputLayers.items():
            if inputName not in input:
                raise 'Network needs the input ' + inputName
            
            inputTensorName = inputLayer.getInputName()
            inputDict = dict(inputDict, **{ inputTensorName: input[inputName] })

        outputTensorNames = []
        for outputLayerName in outputLayerNames:
            if outputLayerName not in self.layers:
                raise 'The layer ' + outputLayerName + ' does not exist'
            outputTensorNames.append(self.layers[outputLayerName].getOutput().name)
        
        results = session.run(tuple(outputTensorNames), feed_dict=inputDict)

        parsedResults = []
        for i in range(0, len(outputLayerNames)):
            parsedResults.append({'name': outputLayerNames[i], 'value': results[i]})
        return parsedResults        
