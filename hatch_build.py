"""Build the comment tool into the package, so installing from source or git just works."""

import subprocess
from pathlib import Path

from hatchling.builders.hooks.plugin.interface import BuildHookInterface

TOOL = Path("src/habit_hooks_csharp/comment-tool")


class CustomBuildHook(BuildHookInterface):
    def initialize(self, version, build_data):
        if self.target_name != "wheel":
            return
        subprocess.run(
            ["dotnet", "build", str(Path(self.root) / TOOL), "-c", "Release", "-nologo", "-v", "quiet"],
            check=True,
        )
