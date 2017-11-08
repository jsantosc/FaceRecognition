""" Common functions used in Tensorflow network classes """

def validatePadding(padding):
    '''Check if the padding is a valid and supported tensorflow padding.'''
    assert padding in ('SAME', 'VALID')
def validateReluType(reluType):
    ''' Check if the relu (Rectifier Linear Unit) type is a valid and supported one '''
    assert reluType in ('RELU', 'PRELU')
