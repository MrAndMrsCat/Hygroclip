import uasyncio as asyncio
from micrologger import MicroLogger
from boiler_controller import BoilerController
from event_timer import EventTimer
from microdiagnostics import MicroDiagnostics

MicroLogger.level = MicroLogger.DEBUG
MicroLogger.directory = "/logs"
MicroLogger.retention_count = 3
logger = MicroLogger("System")

class System(object):
    def __init__(self):
        self.controller = BoilerController("UpperFarm", "mrandmrscat", "192.168.20.4", "192.168.20.2")
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
        

    def _boiler_controller_parameters_changed(self, sender, args):
        timeout = args.get("boiler_enabled_timeout_minutes")
        if timeout is not None:
            self._boiler_timeout_timer.interval_ms = timeout * 1000 * 60

        if self.controller.params["boiler_enabled"]:
            self._boiler_timeout_timer.reset()


    def _boiler_enabled_changed(self, sender, enabled):
        if enabled:
            self._boiler_timeout_timer.start()
        else:
            self._boiler_timeout_timer.stop()
           
            
    def _boiler_timeout_elapsed(self, sender, args):
        timeout_min = self._boiler_timeout_timer.interval_ms / (1000 * 60)
        logger.warn(f"boiler timeout after {timeout_min} minutes")
        self.controller.actuate_boiler(False)


    def _diagnostics_logging_elapsed(self, sender, args):
        logger.debug(f"garbage collector allocation={MicroDiagnostics.free(detailed=True)}")
        logger.debug(f"storage free={MicroDiagnostics.df()}")

system = System()
loop = asyncio.get_event_loop()
loop.run_forever()

logger.flush_stream()
loop.close()
