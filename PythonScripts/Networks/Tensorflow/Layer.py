""" Basic Layer class for all the other layers """

class Layer(object):
    ''' Basic layer functionality for all the neural networks layers '''
    def __init__(self, layerName):
        self.layerName = layerName
        self.output = None

    def setOutput(self, output):
        self.output = output
    
    def getOutput(self):
        return self.output
