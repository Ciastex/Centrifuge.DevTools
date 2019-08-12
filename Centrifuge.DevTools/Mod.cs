using CommandTerminal;
using Reactor.API.Attributes;
using Reactor.API.Configuration;
using Reactor.API.GTTOD;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Storage;
using UnityEngine;
using Logger = Reactor.API.Logging.Logger;

namespace Centrifuge.DevTools
{
    [ModEntryPoint(ModID)]
    public class Mod : MonoBehaviour
    {
        public const string ModID = "com.ciastex.github/Centrifuge.DevTools";

        public const string DumpSceneBasicSettingsKey = "DumpSceneBasic";
        public const string DumpSceneDetailedSettingsKey = "DumpSceneDetailed";
        public const string AddConsoleCommandsSettingsKey = "AddConsoleCommands";

        public const string DumpSceneCommandTrigger = "sc_dump";
        public const string QuitCommandTrigger = "quit";

        private Logger _logger;
        private Settings _settings;
        private FileSystem _fileSystem;

        private Dumper _dumper;

        public void Initialize(IManager manager)
        {
            _fileSystem = new FileSystem();
            _dumper = new Dumper(_fileSystem);
            _logger = new Logger("event");

            GameAPI.TerminalInitialized += GameAPI_TerminalInitialized;

            InitSettings();
            SetUpKeyBinds(manager.Hotkeys);
        }

        private void GameAPI_TerminalInitialized(object sender, System.EventArgs e)
        {
            Terminal.Shell.AddCommand(DumpSceneCommandTrigger, (args) =>
            {
                if (args[0].String == "basic")
                {
                    DumpBasic();
                    Terminal.Log("Basic dump finished.");
                }
                else if (args[0].String == "detailed")
                {
                    DumpDetailed();
                    Terminal.Log("Detailed dump finished.");
                }
                else
                {
                    Terminal.Log("Need 'basic' or 'detailed'.");
                }
            }, 1, 1, "Dumps entire object tree in the current scene into a file inside mod's Data directory.");

            Terminal.Shell.AddCommand(QuitCommandTrigger, (args) =>
            {
                Application.Quit(0);
            }, 0, -1, "Quits the game.");

            Terminal.Autocomplete.Register(DumpSceneCommandTrigger);
            Terminal.Autocomplete.Register(QuitCommandTrigger);
        }

        private void InitSettings()
        {
            _settings = new Settings("config");

            _settings.GetOrCreate(DumpSceneBasicSettingsKey, "LeftControl+F7");
            _settings.GetOrCreate(DumpSceneDetailedSettingsKey, "LeftControl+F8");
            _settings.GetOrCreate(AddConsoleCommandsSettingsKey, true);

            if (_settings.Dirty)
                _settings.Save();
        }

        private void SetUpKeyBinds(IHotkeyManager hotkeys)
        {
            hotkeys.Bind(_settings.GetItem<string>(DumpSceneBasicSettingsKey),
                () =>
                {
                    DumpBasic();
                }
            );

            hotkeys.Bind(_settings.GetItem<string>(DumpSceneDetailedSettingsKey),
                () =>
                {
                    DumpDetailed();
                }
            );
        }

        private void DumpBasic()
        {
            _logger.Info("Performing basic scene dump...");
            _dumper.DumpCurrentScene(false);
        }

        private void DumpDetailed()
        {
            _logger.Info("Performing detailed scene dump...");
            _dumper.DumpCurrentScene(true);
        }
    }
}
