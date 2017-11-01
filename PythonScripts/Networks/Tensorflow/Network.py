""" Module containing basic tensorflow networks deep neural network """

import tensorflow as tf
import InputLayer as il
import ConvolutionalLayer as cl

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
            pass
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

        convLayer = cl.ConvolutionalLayer(self, layerName, self.currentOutputNode.getOutput(), kernelWidth, kernelHeight, kernelChannels,
                        strideHeight, strideWidth, padding, grouping, biased, addRelu, reluType='RELU')
        self.layers = dict(self.layers, **{ layerName: convLayer })
        self.currentOutputNode = convLayer.getOutput()
        return self
a = Network('Pnet')
a.addInputLayer('data', (None,None,None,3))
a.feed('data')
a.addConvolutionalLayer('conv1', 3, 3, 10, 1, 1)
print(a)