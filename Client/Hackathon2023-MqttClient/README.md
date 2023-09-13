## MQTT Desktop Client
This program is a MQTT desktop client which subscribes to a MQQT broker topic and listen to commands. For recognized commands received, it sends to the operational system keyboard commands. It is used currently to interact with Microsoft Teams.

## Supported commands
Check KeyboardHelper.cs

## Supported Operational Systems
- Windows

## Running the client
Make sure to update the MQTT broker configurations in Program.cs

## Sending commands to the MQTT topic
- Commands must be strings, which match with the supported commands in KeyboardHelper.cs
- You can send messages with multiple commands. To do that, separate the commands with comma. E.g., "miconoff,camonoff".