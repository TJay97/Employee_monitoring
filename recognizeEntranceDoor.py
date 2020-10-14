# USAGE
# python recognizeEntranceDoor.py --encodings encodings.pickle --input videos/Employee_In.mp4 --model fashion.model --labelbin mlb.pickle

# import the necessary packages Face Detection
import face_recognition
import argparse
import imutils
import pickle
import time
import cv2
import sys
import logging as log
import datetime as dt
from time import sleep

from keras.preprocessing.image import img_to_array
from keras.models import load_model
import numpy as np
import imutils
import os


# Supplimentry method for Dress Detection
def load_Img(frame):
	# load the frame and resize
	img = imutils.resize(frame, width=750)
	# convert to array
	img = img_to_array(img)
	# reshape into a single sample with 1 channel
	img = img.reshape(1, 28, 28, 1)
	# prepare pixel data
	img = img.astype('float32')
	img = img / 255.0
	return img

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()

#FaceDetection Arguments
ap.add_argument("-e", "--encodings", required=True,
	help="path to serialized db of facial encodings")
ap.add_argument("-i", "--input", required=True,
	help="path to input video")
ap.add_argument("-o", "--output", type=str,
	help="path to output video")
ap.add_argument("-y", "--display", type=int, default=1,
	help="whether or not to display output frame to screen")
ap.add_argument("-d", "--detection_method", type=str, default="cnn",
	help="face detection model to use: either `hog` or `cnn`")

#Dress Detection Arguments
ap.add_argument("-m", "--model", required=True,
	help="path to trained model model")
ap.add_argument("-l", "--labelbin", required=True,
	help="path to label binarizer")
args = vars(ap.parse_args())

if args["input"] == "videos/Employee_In.mp4":
	log.basicConfig(filename='webcam_FrontCamera.log',level=log.INFO)
else:
	log.basicConfig(filename='webcam_FrontCameraOut.log',level=log.INFO)

# load the known faces and embeddings
print("[INFO] loading encodings...")
data = pickle.loads(open(args["encodings"], "rb").read())

# initialize the pointer to the video file and the video writer
print("[INFO] processing video...")
stream = cv2.VideoCapture(args["input"])
writer = None
fps = stream.get(cv2.CAP_PROP_FPS)
print(fps)

# loop over frames from the video file stream
while True:
	print (1)
	# grab the next frame
	(grabbed, frame) = stream.read()

	# if the frame was not grabbed, then we have reached the
	# end of the stream
	if not grabbed:
		break

	# convert the input frame from BGR to RGB then resize it to have
	# a width of 750px (to speedup processing)
	rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
	rgb = imutils.resize(frame, width=750)
	r = frame.shape[1] / float(rgb.shape[1])

	# detect the (x, y)-coordinates of the bounding boxes
	# corresponding to each face in the input frame, then compute
	# the facial embeddings for each face
	boxes = face_recognition.face_locations(rgb,
		model=args["detection_method"])
	encodings = face_recognition.face_encodings(rgb, boxes)
	names = []
	detectedDress = []

	# loop over the facial embeddings
	for encoding in encodings:
		# attempt to match each face in the input image to our known
		# encodings
		matches = face_recognition.compare_faces(data["encodings"],
			encoding)
		name = "Unknown"

		# check to see if we have found a match
		if True in matches:
			# find the indexes of all matched faces then initialize a
			# dictionary to count the total number of times each face
			# was matched
			matchedIdxs = [i for (i, b) in enumerate(matches) if b]
			counts = {}

			# loop over the matched indexes and maintain a count for
			# each recognized face face
			for i in matchedIdxs:
				name = data["names"][i]
				counts[name] = counts.get(name, 0) + 1

			# determine the recognized face with the largest number
			# of votes (note: in the event of an unlikely tie Python
			# will select first entry in the dictionary)
			name = max(counts, key=counts.get)			
			
			# Dress Detection ##############################################################			
			# pre-process the image for classification
			image = cv2.resize(frame, (96, 96))
			image = image.astype("float") / 255.0
			image = img_to_array(image)
			image = np.expand_dims(image, axis=0)

			model = load_model(args["model"])
			mlb = pickle.loads(open(args["labelbin"], "rb").read())

			proba = model.predict(image)[0]
			idxs = np.argsort(proba)[::-1][:1]
			
			# loop over the indexes of the high confidence class labels
			for (i, j) in enumerate(idxs):
				# build the label and draw the label on the image
				# update the list of Dress
				detectedDress.append(mlb.classes_[j])
				print(mlb.classes_[j])
			
			# Dress Detection ##############################################################v

		# update the list of names
		names.append(name)

	log.info("Faces:"+str(len(encodings)))
	log.info("GetTime:"+str(dt.datetime.now()))
	log.info("Timestamp: " + str(stream.get(cv2.CAP_PROP_POS_MSEC)))
	log.info("Names:" + str(names))
	log.info("Dresses:" + str(detectedDress))
	log.info("--------------------------------------------EndLine")


# close the video file pointers
stream.release()
print ("Done")

# check to see if the video writer point needs to be released
if writer is not None:
	writer.release()
