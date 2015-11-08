#!python
import fileinput
import sys
import os
import pdb

def main():
	oldTime = os.path.getmtime("temp.txt") 
	print(oldTime)
	print("======")
	i = 0
	while True:
		try:
			pdb.set_trace()
			currTime = os.path.getmtime("temp.txt")
			if (currTime > oldTime):
				print("Here")
				oldTime = currTime
				i += 1
			if (i == 5):
				sys.exit()
		except:
			pass
main()
