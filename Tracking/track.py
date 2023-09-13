import numpy as np
import track_hand as htm
import time
import autopy
import cv2
import urllib
import mqtt_wrapper as mm
import time
import sys


class gensture:
    def __init__(self, show, local_cam=False) -> None:
        self.url = "http://192.168.0.98/capture"
        self.wCam = 640
        self.hCam = 480
        self.frameR = 100
        self.smoothening = 7
        self.pTime = 0
        self.plocX = 0
        self.plocY = 0
        self.clocX = 0
        self.clocY = 0
        self.detector = htm.handDetector(maxHands=1)
        self.wScr, self.hScr = autopy.screen.size()
        self.last_cmd = ""
        self.cmd_delay = 0
        self.def_delay = 3
        self.show = show
        self.local_cam = local_cam
        self.mqtt = mm.mqtt_wrapper(
            "***",
            0,
            "***",
            "***",
            True,
        )

        self.mqtt.publish_multi(
            [{"topic": "hivemqdemo/commands", "payload": "welcome"}]
        )

        if self.local_cam:
            self.cap = cv2.VideoCapture(0)
            self.cap.set(3, self.wCam)
            self.cap.set(4, self.hCam)
            self.def_delay = 10

    def handle_command(self, cmd, doDelay):
        if self.last_cmd != cmd:
            self.cmd_delay = 0
            self.last_cmd = cmd
        elif doDelay:
            self.cmd_delay = self.cmd_delay + 1
            if self.cmd_delay == self.def_delay:
                print(self.last_cmd)
                self.publish_command("hivemqdemo/commands", self.last_cmd)

    def publish_command(self, topic, cmds):
        msgs = [{"topic": topic, "payload": cmds}]
        self.mqtt.publish_multi(msgs)

    def draw_circle(self, img, points):
        for point in points:
            cv2.circle(img, point, 15, (255, 0, 255), cv2.FILLED)

    def monitor(self):
        while True:
            # 1. Find hand Landmarks
            fingers = [0, 0, 0, 0, 0]
            # success, img = cap.read()
            img = None

            if self.local_cam:
                success, img = self.cap.read()
            else:
                img_resp = urllib.request.urlopen(self.url)
                imgnp = np.array(bytearray(img_resp.read()), dtype=np.uint8)
                img = cv2.imdecode(imgnp, -1)

            img = self.detector.findHands(img)
            lmList, bbox = self.detector.findPosition(img)

            # 2. Get the tip of the index and middle fingers
            if len(lmList) != 0:
                x1, y1 = lmList[8][1:]
                x2, y2 = lmList[12][1:]
                x3, y3 = lmList[16][1:]
                x4, y4 = lmList[20][1:]
                # print(x1, y1, x2, y2)

                # 3. Check which fingers are up
                fingers = self.detector.fingersUp()

            cv2.rectangle(
                img,
                (self.frameR, self.frameR),
                (self.wCam - self.frameR, self.hCam - self.frameR),
                (255, 0, 255),
                2,
            )

            if (
                fingers[1] == 1
                and fingers[2] == 0
                and fingers[3] == 0
                and fingers[4] == 0
            ):
                self.draw_circle(img, [(x1, y1)])
                self.handle_command("miconoff", True)

            elif (
                fingers[1] == 1
                and fingers[2] == 1
                and fingers[3] == 0
                and fingers[4] == 0
            ):
                self.draw_circle(img, [(x1, y1), (x2, y2)])
                self.handle_command("camonoff", True)

            elif (
                fingers[1] == 1
                and fingers[2] == 1
                and fingers[3] == 1
                and fingers[4] == 0
            ):
                self.draw_circle(img, [(x1, y1), (x2, y2), (x3, y3)])
                self.handle_command("miconoff;camonoff", True)

            elif (
                fingers[1] == 1
                and fingers[2] == 1
                and fingers[3] == 1
                and fingers[4] == 1
            ):
                self.draw_circle(img, [(x1, y1), (x2, y2), (x3, y3), (x4, y4)])
                self.handle_command("hello", True)
            else:
                self.handle_command("", False)

            cTime = time.time()
            fps = 1 / (cTime - self.pTime)
            self.pTime = cTime
            cv2.putText(
                img, str(int(fps)), (20, 50), cv2.FONT_HERSHEY_PLAIN, 3, (255, 0, 0), 3
            )
            if self.show:
                cv2.imshow("Image", img)
            cv2.waitKey(1)


show = False
local = False
for a in sys.argv:
    if a == "show":
        show = True
    elif a == "local":
        local = True

g = gensture(show, local)

g.monitor()
