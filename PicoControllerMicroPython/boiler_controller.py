import uasyncio as asyncio
import time
import machine
from micrologger import MicroLogger
from wifi_tcp_server import WiFiTCPServer
from stepper_motor import StepperMotor
from onboard_led import OnboardLED
from event_handler import EventHandler
from microdiagnostics import MicroDiagnostics
from settings import Settings

logger = MicroLogger("BoilerInterface")
settings = Settings()

class BoilerController(object):
    def __init__(self, ssid: str, password: str, static_ip: str, gateway: str):
        self.motor = StepperMotor(drive_pattern = "full", steps_per_sec=50)
        self.motor.move_completed.add(self._motor_move_completed)
        self.led = OnboardLED()
        self.params: dict = {
            "state" : "Uninitialized",
            "boiler_enabled" : False,
            "boiler_enabled_timeout_minutes" : 30,
            "temperature" : float("nan"),
            "temperature_setpoint" : float("nan"),
            "humidity" : float("nan"),
            "led_comms_falsh_count" : 2,
            "motor_speed" : 50,
            "motor_home_steps" : 300,
            "motor_boiler_enable_steps" : 200,
            }
        self.parameters_changed: EventHandler = EventHandler()
        self.state_changed: EventHandler = EventHandler()
        self.boiler_enabled_changed: EventHandler = EventHandler()
        self.server = WiFiTCPServer(ssid, password, port=42440, static_ip=static_ip, gateway=gateway)
        self.server.termination_byte = b'\x04'
        self.server.bytes_received.add(self.bytes_received)
        self.server.start()


    def status(self) -> str:
        tm = time.localtime()
        self.params["time"] = f"{tm[0]:04d}-{tm[1]:02d}-{tm[2]:02d} {tm[3]:02d}:{tm[4]:02d}:{tm[5]:02d}"
        self.params["garbage_collector"] = MicroDiagnostics.free(detailed=True)
        self.params["storage_free"] = MicroDiagnostics.df()
        return f"status|" + ",".join('='.join((key,str(val))) for (key,val) in self.params.items())


    def bytes_received(self, sender, message: bytes):
        cmd_args = message.decode("utf-8").split('|')
        logger.verb(f"Rx << {cmd_args}")
        self.led.flash(self.params["led_comms_falsh_count"])
        cmd = cmd_args[0]

        response = f"ack|{cmd}"

        if cmd == "req_status":
             response = self.status()

        elif cmd == "initialize":
            self.initialize()

        elif cmd == "move":
            self.motor.move_steps(int(cmd_args[1]))

        elif cmd == "set_parameters":
            params = {}
            for key_value in cmd_args[1].split(','):
                key, value = key_value.split('=')

                # convert if already exists
                typ = type(self.params.get(key))
                if typ == bool:
                    value = value == "True" or value == "true"
                elif typ == int:
                    value = int(value)
                elif typ == float:
                    value = float(value)

                params[key] = value
                
            self.set_parameters(params)

        else:
            response = f"nak|does not understand: {cmd_args}"

        self.send_string(response)


    def send_string(self, message: str):
        logger.verb(f"Tx >> {message}")
        self.server.send_message(message.encode("utf-8"))
        self.led.flash(self.params["led_comms_falsh_count"])

    def _send_status(self):
        self.send_string(self.status())

    def set_parameters(self, parameters: dict):
        logger.verb(f"set_parameters {parameters}")
        for key, value in parameters.items():
            if key == "boiler_enabled":
                self.actuate_boiler(value)
                continue # motor move complete callback should set self.params[key] = value

            if key == "motor_speed":
                self.motor.steps_per_sec = value

            if key == "datetime":
                # (year, month, day, weekday, hours, minutes, seconds, subseconds)
                dt = tuple([int(x) for x in value.split(' ')])
                logger.debug(f"set datetime {dt}")
                machine.RTC().datetime(dt)
                continue # discard

            if key == "enable_ui":
                settings.set("enable_ui", value)

            logger.verb(f"set_parameter {key}={value}")
            self.params[key] = value
        self.parameters_changed.invoke(self, parameters)


    def _on_state_changed(self, state: str):
        logger.info(f"state changed: {state}")
        self.params["state"] = state
        self._send_status()
        # self.state_changed.invoke(self, state)


    def _on_boiler_enabled_changed(self, enabled: bool):
        logger.debug(f"boiler_enabled: {enabled}")
        self.params["boiler_enabled"] = enabled
        self.led.value(enabled)
        self._send_status()
        self.boiler_enabled_changed.invoke(self, enabled)


    def initialize(self):
        if self.params["state"] != "Initializing":
            self._on_state_changed("Initializing")
            self._home_motor()
        

    def actuate_boiler(self, enable: bool):
        currently_enabled = self.params["boiler_enabled"]
        fsm = self.params["state"]
        logger.debug(f"actuate_boiler(enable={enable}), currently_enabled={currently_enabled}, fsm={fsm}")
        if enable != currently_enabled and fsm != "ActuatingBoiler":
            self._on_state_changed("ActuatingBoiler")
            steps = self.params["motor_boiler_enable_steps"]
            self.motor.move_steps(steps if enable else (0 - steps))


    def _home_motor(self):
        logger.debug("home_motor()")
        self.motor.move_steps(0-self.params["motor_home_steps"])
        self.led.off()


    def _motor_move_completed(self, sender, args):
        state = self.params["state"]
        logger.debug(f"_motor_move_completed(args={args}), state={state}")

        if state == "Initializing":
            self._on_state_changed("Idle")

        elif state == "ActuatingBoiler":
            self._on_state_changed("Idle")
            self._on_boiler_enabled_changed(not self.params["boiler_enabled"])
