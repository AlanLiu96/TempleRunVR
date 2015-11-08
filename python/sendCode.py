import bluetooth
import sys

target_name = "SAMSUNG-SM-G920A"
target_address = "E0:99:71:52:B2:71" #Samsung
# target_address = "C0:EE:FB:34:06:DB" # my OPO

uuid = "bd7300a0-85df-11e5-9353-0002a5d5c51b"
service_matches = bluetooth.find_service( uuid = uuid, address = target_address )


if len(service_matches) == 0:
    print("couldn't find the SampleServer service =(")
    sys.exit(0)

first_match = service_matches[0]
port = first_match["port"]
name = first_match["name"]
host = first_match["host"]

print("connecting to \"%s\" on %s" % (name, host))

# Create the client socket
sock=bluetooth.BluetoothSocket( bluetooth.RFCOMM )
sock.connect((host, port))

while True:
	signal=sys.stdin.read(1)
	dumpNewLine=sys.stdin.read(1)
	if dumpNewLine!= '\n':
		break
	sock.send(signal)

sock.close()
