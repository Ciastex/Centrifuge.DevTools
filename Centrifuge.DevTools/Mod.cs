using Reactor.API.Attributes;
using Reactor.API.Configuration;
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

        private Logger _logger;
        private Settings _settings;
        private FileSystem _fileSystem;

        private Dumper _dumper;

        public void Initialize(IManager manager)
        {
            _fileSystem = new FileSystem();
            _dumper = new Dumper(_fileSystem);
            _logger = new Logger("event");

            InitSettings();
            SetUpKeyBinds(manager.Hotkeys);
        }

        private void InitSettings()
        {
            _settings = new Settings("binds");

            _settings.GetOrCreate(DumpSceneBasicSettingsKey, "LeftControl+F7");
            _settings.GetOrCreate(DumpSceneDetailedSettingsKey, "LeftControl+F8");

            if (_settings.Dirty)
                _settings.Save();
        }

        private void SetUpKeyBinds(IHotkeyManager hotkeys)
        {
            hotkeys.Bind(_settings.GetItem<string>(DumpSceneBasicSettingsKey),
                () =>
                {
                    _logger.Info("Performing basic scene dump...");
                    _dumper.DumpCurrentScene(false);
                }
            );

            hotkeys.Bind(_settings.GetItem<string>(DumpSceneDetailedSettingsKey),
                () =>
                {
                    _logger.Info("Performing detailed scene dump...");
                    _dumper.DumpCurrentScene(true);
                }
            );
        }
    }
}
