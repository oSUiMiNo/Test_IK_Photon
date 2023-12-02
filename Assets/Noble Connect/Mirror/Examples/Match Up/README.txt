Example scene that demonstrates the use of Match Up and the NobleNetworkManager to connect and send data.

Requires Match Up. Match Up is available on the Asset Store or for free with any paid plan.

ExampleMatchUpNetworkManager.cs shows how to extend from the NobleNetworkManager to implement your custom 
networking behaviour and utilize the Match Up plugin for matchmaking.

ExamplePunchthroughHUD is used to provide a basic GUI interface.

The buttons and text boxes can be used to host a server or connect as a client. When running as a host, a
match is automatically created. When running as a client, the a list of all matches is retrieved and
the client connects to the host of the first match in the list.

When a client connects, a player will be spawned that can be moved around with the arrow keys.

The connection type will be displayed on the client:
DIRECT - The connection was made directly to the host's IP.
PUNCHTHROUGH - The connection was made to an address on the host's router discovered via punchthrough.
RELAY - The connection is using the Noble Connect relays.