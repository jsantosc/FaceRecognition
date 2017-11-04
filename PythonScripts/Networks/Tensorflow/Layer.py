""" Basic Layer class for all the other layers """

class Layer(object):
    ''' Basic layer functionality for all the neural networks layers '''
    def __init__(self, layerName):
        self.layerName = layerName
        self.output = None
        self.type = 'Layer'

    def setOutput(self, output):
        self.output = output
    
    def getOutput(self):
        return self.output

    def setType(self, type):
        self.type = type

    def getType(self):
        return self.type
    
    def loadManualWeights(self, session, weights):
        pass
