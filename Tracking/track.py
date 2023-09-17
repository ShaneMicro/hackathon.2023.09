import numpy as np
import track_hand as htm
import time
from screeninfo import get_monitors
import cv2
import urllib
import mqtt_wrapper as mm
import sys


class gesture_info:
    def __init__(self, gesture_pattern, command, hand="") -> None:
        self.hand = hand
        self.gesture_pattern = gesture_pattern
        self.command = command


class gesture_engine:
    def __init__(self, show, local_cam=False, stream=False, no_mqtt=False) -> None:
        monitors = get_monitors()

        self.url = (
            "http://192.168.0.98:81/stream" if stream else "http://192.168.0.98/capture"
        )
        self.camera_width = 640
        self.camera_height = 480
        self.framerate = 100
        self.smoothening = 7
        self.process_time = 0
        self.detector = htm.handDetector(maxHands=1)
        self.screen_width = monitors[0].width
        self.screen_height = monitors[0].height
        self.last_cmd = ""
        self.cmd_delay = 0
        self.def_delay = 3
        self.show = show
        self.local_cam = local_cam
        self.stream = stream
        self.no_mqtt = no_mqtt
        self.mqtt = mm.mqtt_wrapper(
            "***",
            0,
            "***",
            "***",
            True,
        )
        self.gestures = [
            gesture_info([0, 1, 0, 0, 0], "miconoff"),
            gesture_info([0, 1, 1, 0, 0], "camonoff"),
            gesture_info([0, 1, 0, 0, 1], "miconoff;camonoff"),
            gesture_info([0, 1, 1, 1, 1], "raisehand"),
            gesture_info([1, 0, 0, 0, 1], "leavecall"),
        ]

        if not self.no_mqtt:
            self.mqtt.publish_multi(
                [{"topic": "hivemqdemo/commands", "payload": "welcome"}]
            )

        if self.local_cam:
            self.cap = cv2.VideoCapture(0)
            self.cap.set(3, self.camera_width)
            self.cap.set(4, self.camera_height)
            self.def_delay = 10

        if self.stream:
            self.cap = cv2.VideoCapture(self.url)
            self.cap.set(3, self.camera_width)
            self.cap.set(4, self.camera_height)

    def handle_command(self, command, delay_command):
        if self.last_cmd != command:
            self.cmd_delay = 0
            self.last_cmd = command
        elif delay_command:
            self.cmd_delay = self.cmd_delay + 1
            if self.cmd_delay == self.def_delay:
                print(self.last_cmd)
                self.publish_command("hivemqdemo/commands", self.last_cmd)

        return False if command == "" else True

    def publish_command(self, topic, command):
        msgs = [{"topic": topic, "payload": command}]
        if not self.no_mqtt:
            self.mqtt.publish_multi(msgs)
        else:
            print(msgs)

    def draw_circle(self, img, points):
        for point in points:
            cv2.circle(img, point, 15, (255, 0, 255), cv2.FILLED)

    def highlight_fingers(self, img, landmarks, fingers):
        if len(landmarks) == 0:
            return

        points = [
            landmarks[4][1:],
            landmarks[8][1:],
            landmarks[12][1:],
            landmarks[16][1:],
            landmarks[20][1:],
        ]
        draw_points = []
        for i, finger in enumerate(fingers):
            if finger == 1:
                draw_points.append(points[i])

        self.draw_circle(img, draw_points)

    def handle_gesture(self, fingers, hand):
        for gesture in self.gestures:
            if (gesture.hand == "" or gesture.hand == hand) and np.array_equal(
                fingers, gesture.gesture_pattern
            ):
                return self.handle_command(gesture.command, True)

        return self.handle_command("", False)

    def do_frame_rate(self, img):
        cTime = time.time()
        fps = 1 / (cTime - self.process_time)
        self.process_time = cTime
        cv2.putText(
            img,
            str(int(fps)),
            (20, 50),
            cv2.FONT_HERSHEY_PLAIN,
            3,
            (255, 0, 0),
            3,
        )

    def monitor(self):
        while True:
            # 1. Find hand Landmarks
            fingers = [0, 0, 0, 0, 0]
            hand_raised = ""
            # success, img = cap.read()
            img = None

            if self.local_cam or self.stream:
                try:
                    _, img = self.cap.read()
                except:
                    continue
            else:
                img_resp = urllib.request.urlopen(self.url)
                imgnp = np.array(bytearray(img_resp.read()), dtype=np.uint8)
                img = cv2.imdecode(imgnp, -1)

            if img is not None:
                img = self.detector.findHands(img)
                landmark_list, _ = self.detector.findPosition(img)

                if len(landmark_list) != 0:
                    fingers, hand_raised = self.detector.fingersUp()

                if self.handle_gesture(fingers, hand_raised):
                    self.highlight_fingers(img, landmark_list, fingers)

                self.do_frame_rate(img)

                if self.show:
                    cv2.imshow("Frame", img)

            cv2.waitKey(1)


show = False
local = False
stream = False
no_mqtt = False

for a in sys.argv:
    if a == "show":
        show = True
    elif a == "local":
        local = True
    elif a == "stream":
        stream = True
    elif a == "no_mqtt":
        no_mqtt = True

gesture_processor = gesture_engine(show, local, stream, no_mqtt)

gesture_processor.monitor()
