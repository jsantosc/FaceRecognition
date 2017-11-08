""" Module that contains a PNET network of the Mtcc algorithm """
import Networks.Tensorflow.Network as n

class PNet(n.Network):

    def __init__(self, trainable=True):
        super(PNet, self).__init__('PNet', trainable)

    def createNetwork(self):
        self.addInputLayer('data', (None, None, None, 3))
        self.feed('data')
        self.addConvolutionalLayer('conv1', 3, 3, 10, 1, 1, padding='VALID', addRelu=True, reluType='PRELU').addMaxPooling(
            'pool1', 2, 2, 2, 2).addConvolutionalLayer(
            'conv2', 3, 3, 16, 1, 1, padding='VALID', addRelu=True, reluType='PRELU').addConvolutionalLayer(
            'conv3', 3, 3, 32, 1, 1, padding='VALID', addRelu=True, reluType='PRELU').addConvolutionalLayer(
            'conv4-1', 1, 1, 2, 1, 1, padding='VALID', addRelu=False).addSoftmax(
            'prob1', 3)
        self.feed('conv3')
        self.addConvolutionalLayer('conv4-2', 1, 1, 4, 1, 1, addRelu=False)
    
    def loadManualWeights(self, session, weights):
        for key,value in weights.items():
            if key not in self.layers:
                raise 'Not valid weight'
            
            self.layers[key].loadManualWeights(session, value)
