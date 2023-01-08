import time

from LCD.lcd_driver import LCDDriver
from LCD.lcd_text import LCDTextWriter
from LCD.lcd_image import LCDImage
from LCD.lcd_drawing import LCDDrawing
from LCD.ui_indicator import Indicator
from LCD.lcd_driver import LCDDriver

class BoilerUIHome(object):

    def __init__(self):
        self.indicators = {}
        self.text_scale: str = "full"
        self.text_coordinates: dict = {}
        self.text_values: dict = {}


    def initialize(self, lcd_driver):
        self._driver = lcd_driver
        self._drawing = LCDDrawing()
        self._drawing.initialize(self._driver)
        self._text = LCDTextWriter()
        self._text.initialize(self._driver)
        

    def build_controls(self):
        spacing = 28
        x, y = 25, -10

        self.indicators["wifi_connected"] = Indicator(x=x, y=(y:=y+spacing), text="WiFi Connected")
        self.indicators["clients_connected"] = Indicator(x=x, y=(y:=y+spacing), text="Server Connected")
        self.indicators["control_enabled"] = Indicator(x=x, y=(y:=y+spacing), text="Control Enabled")
        self.indicators["boiler_enabled"] = Indicator(x=x, y=(y:=y+spacing), text="Boiler Enabled")

        for indicator in self.indicators.values():
            indicator.text_scale = "full"
            indicator.border = 3
            indicator.size = 24


    def draw(self):
        self._driver.clear_screen()

        # border
        self.set_boiler_enabled(False)

        # static text
        spacing = 28
        x, y = 30, 110
        x_value = x - 5 + self._text.CHAR_WIDTH * 12 # chars

        self._text.forecolor = 255, 0, 0
        self._text.write_at(x=x, y=(y:=y+spacing), string="Temperature     C", scale=self.text_scale)
        self.text_coordinates["temperature"] = (x_value, y)

        self._text.forecolor = 255, 127, 0
        self._text.write_at(x=x, y=(y:=y+spacing), string="   Setpoint     C", scale=self.text_scale)
        self.text_coordinates["temperature_setpoint"] = (x_value, y)

        self._text.forecolor = 0, 0, 255
        self._text.write_at(x=x, y=(y:=y+spacing), string="   Humidity     %", scale=self.text_scale)
        self.text_coordinates["humidity"] = (x_value, y)

        # controls / components
        for control in self.indicators.values():
            control.draw()


    def set_parameters(self, parameters: dict):
        for key, value in parameters.items():
            coordinates = self.text_coordinates.get(key)
            if coordinates is not None:
                string_value = f"{value}"[:4]
                if self.text_values.get(value) != string_value:
                    x, y = coordinates
                    self._text.forecolor = 255, 255, 255
                    self._text.write_at(x=x, y=y, string=string_value, scale=self.text_scale)
                self.text_values[key] = string_value

            if key == "control_enabled":
                self.indicators["control_enabled"].enable(value=="True")


    def set_boiler_enabled(self, enabled: bool):
        self.indicators["boiler_enabled"].enable(enabled)
        border_color = (0, 255, 0) if enabled else (64, 64, 64)
        self.set_border(border_color)


    def set_border(self, color: tuple[int, int, int]):
        self._drawing.rectangle(0, 319, 0, 239, 8, color)
              

if __name__ == "__main__":
    import random
    pico = False

    if pico:
        from LCD.pico_spi_display_interface import PICOSPIDisplayInterface
        interface = PICOSPIDisplayInterface()
    else:
        from LCD.rp_spi_display_interface import RPSPIDisplayInterface
        interface = RPSPIDisplayInterface()

    driver = LCDDriver(interface)
    driver.initialize()
    ui = BoilerUIHome()
    ui.initialize(driver)
    ui.build_controls()
    ui.draw()

    for i in range(100):
        for indicator in ui.indicators.values():
            indicator.enable(random.getrandbits(1))

        for parameter in ui.text_coordinates.keys():
            ui.set_parameters({ parameter : f"{random.random() * 50}"})

        ui.set_boiler_enabled(random.getrandbits(1))

        time.sleep(0.5)
 