Example scene that demonstrates how to utilize the Noble Connect relay and punchthrough services with Netcode for GameObjects and Match Up

The buttons can be used to host a server and create a match, or fetch the match list and join one as a client. 

When a client connects, a player will be spawned that can be moved around with the arrow keys.

The connection type will be displayed on the client:
DIRECT - The connection was made directly to the host's IP.
PUNCHTHROUGH - The connection was made to an address on the host's router discovered via punchthrough.
RELAY - The connection is using the Noble Connect relays.