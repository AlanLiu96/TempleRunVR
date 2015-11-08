import bluetooth
import sys

target_name = "SAMSUNG-SM-G920A"
target_address = "E0:99:71:52:B2:71"

nearby_devices = bluetooth.discover_devices()

if target_address not in nearby_devices:
	print "could not find target bluetooth device nearby"
else:
	print "connected!"
port = 3

sock=bluetooth.BluetoothSocket(bluetooth.RFCOMM)

sock.connect((target_address,port))


while True:
	signal=sys.stdin.read(1)
	dumpNewLine=sys.stdin.read(1)
	sock.send(signal)

sock.close()
