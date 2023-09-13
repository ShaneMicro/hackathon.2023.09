import random
import time
from paho.mqtt import client as mqtt_client
import ssl
import paho.mqtt.publish as publish

FIRST_RECONNECT_DELAY = 1
RECONNECT_RATE = 2
MAX_RECONNECT_COUNT = 12
MAX_RECONNECT_DELAY = 60


class mqtt_wrapper:
    @property
    def connected(self):
        return self._connected

    def __init__(self, broker, port, user, pwd, tls=False) -> None:
        self._broker = broker
        self._port = port
        self._user = user
        self._pwd = pwd
        self._id = f"python-mqtt-{random.randint(0, 1000)}"
        self._client = None
        self._connected = False
        self._tls = tls

    def connect(self):
        print(f"Connecting to mqtt {self._broker} as {self._id}")
        self._connected = False
        self._client = mqtt_client.Client(self._id)
        self._client.username_pw_set(self._user, self._pwd)
        self._client.on_connect = self.on_connect
        self._client.disconnect = self.on_disconnect
        self._client.connect_async(self._broker, self._port)

    def on_connect(self, client, userdata, flags, rc):
        if rc == 0:
            print("Connected to MQTT Broker!")
            self._connected = True
        else:
            print("Failed to connect, return code %d\n", rc)

    def on_disconnect(self, client, userdata, rc):
        print("Disconnected from mqtt, attempting reconnect.")
        self._connected = False
        reconnect_count, reconnect_delay = 0, FIRST_RECONNECT_DELAY
        while reconnect_count < MAX_RECONNECT_COUNT:
            time.sleep(reconnect_delay)

            try:
                client.reconnect()
                self._connected = True
                return
            except Exception as err:
                print("%s. Reconnect failed. Retrying...", err)

            reconnect_delay *= RECONNECT_RATE
            reconnect_delay = min(reconnect_delay, MAX_RECONNECT_DELAY)
            reconnect_count += 1

        print("Reconnect failed after %s attempts. Exiting...", reconnect_count)

    def publish_multi(self, msgs):
        auth = {"username": self._user, "password": self._pwd}

        if self._tls:
            sslSettings = ssl.SSLContext(mqtt_client.ssl.PROTOCOL_TLS)

            publish.multiple(
                msgs,
                hostname=self._broker,
                port=self._port,
                auth=auth,
                tls=sslSettings,
                protocol=mqtt_client.MQTTv31,
            )
        else:
            publish.multiple(
                msgs,
                hostname=self._broker,
                port=self._port,
                auth=auth,
                protocol=mqtt_client.MQTTv31,
            )
