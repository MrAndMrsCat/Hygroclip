import uasyncio as asyncio
from micrologger import MicroLogger
from boiler_controller import BoilerController
from event_timer import EventTimer
from microdiagnostics import MicroDiagnostics
from LCD.pico_spi_display_interface import PICOSPIDisplayInterface
from LCD.lcd_driver import LCDDriver
from boiler_ui import BoilerUIHome

MicroLogger.level = MicroLogger.DEBUG
MicroLogger.directory = "/logs"
MicroLogger.retention_count = 3
logger = MicroLogger("System")

class System(object):
    def __init__(self, ui: bool = False):
        self.controller = BoilerController("TickleMePink", "hesaidlockthedoor", "192.168.30.4", "192.168.30.1")
        self.controller.parameters_changed.add(self._boiler_controller_parameters_changed)
        self.controller.boiler_enabled_changed.add(self._boiler_enabled_changed)

        self.diagnostics_logging_timer = EventTimer(interval_ms=600000)
        self.diagnostics_logging_timer.elapsed.add(self._diagnostics_logging_elapsed)
        self.diagnostics_logging_timer.name = "diagnostics log"
        self.diagnostics_logging_timer.start()

        timeout = 30 * 60 * 1000
        self._boiler_timeout_timer = EventTimer(interval_ms=timeout)
        self._boiler_timeout_timer.elapsed.add(self._boiler_timeout_elapsed)
        self._boiler_timeout_timer.name = "boiler timeout"
        
        if ui:
            self._initialize_ui()
        else:
            self.ui = None


    def _boiler_controller_parameters_changed(self, sender, args):
        timeout = args.get("boiler_enabled_timeout_minutes")
        if timeout is not None:
            self._boiler_timeout_timer.interval_ms = timeout * 1000 * 60

        if self.controller.params["boiler_enabled"]:
            self._boiler_timeout_timer.reset()

        self._set_ui_paramters(args)


    def _boiler_enabled_changed(self, sender, enabled):
        if enabled:
            self._boiler_timeout_timer.start()
        else:
            self._boiler_timeout_timer.stop()

        self._set_ui_indicator("boiler_enabled", enabled)
           
            
    def _boiler_timeout_elapsed(self, sender, args):
        timeout_min = self._boiler_timeout_timer.interval_ms / (1000 * 60)
        logger.warn(f"boiler timeout after {timeout_min} minutes")
        self.controller.actuate_boiler(False)


    def _diagnostics_logging_elapsed(self, sender, args):
        logger.debug(f"garbage collector allocation={MicroDiagnostics.free(detailed=True)}")
        logger.debug(f"storage free={MicroDiagnostics.df()}")


    def _initialize_ui(self):
        self._display_driver = LCDDriver(PICOSPIDisplayInterface())
        self._display_driver.initialize()
        self.ui = BoilerUIHome()
        self.ui.initialize(self._display_driver)
        self.ui.build_controls()
        self.ui.draw()

        self.controller.server.connection_changed.add(self._server_connection_changed)

    def _set_ui_indicator(self, name: str, value: bool):
        if self.ui is not None:
            try:
                self.ui.indicators[name].enable(value)
            except Exception as ex:
                logger.exception(ex)


    def _set_ui_paramters(self, parameters: dict):
        if self.ui is not None:
            try:
                self.ui.set_parameters(parameters)
            except Exception as ex:
                logger.exception(ex)


    def _server_connection_changed(self, sender, args):
        for name, value in args.items():
            self._set_ui_indicator(name, value)


system = System(ui=False)

loop = asyncio.get_event_loop()
loop.run_forever()

logger.flush_stream()
loop.close()
