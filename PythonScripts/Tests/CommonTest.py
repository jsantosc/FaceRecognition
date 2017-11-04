""" Module with common functionality for testing """

from scipy import misc
import cv2
import numpy as np

def imload(path):
    img = misc.imread(path)
    return img;

def imresample(img, newWidth, newHeight):
    imgResized = cv2.resize(img, (newWidth, newHeight), interpolation=cv2.INTER_AREA)
    return imgResized

def getTestImageHWC(path, newWidth, newHeight):
    img = imload(path)
    imgResized = imresample(img, newWidth, newHeight)
    imgResized = (imgResized-127.5)*0.0078125
    imgResized = np.expand_dims(imgResized, 0)
    return imgResized

def getTestImageWHC(path, newWidth, newHeight):
    img = getTestImageHWC(path, newWidth, newHeight)
    img = np.transpose(img, (0,2,1,3))
    return img
