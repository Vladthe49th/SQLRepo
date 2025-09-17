using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public class MissionManager
    {
        private readonly List<IMission> _missions = new();
        private readonly ILogger _logger;

        public MissionManager(ILogger logger)
        {
            _logger = logger;
        }

        public void AddMission(IMission mission)
        {
            _missions.Add(mission);
            _logger.Log($"[MissionManager] Added mission: {mission.Name}");
        }

        public IReadOnlyList<IMission> Missions => _missions.AsReadOnly();

     
        public void LogMissionResult(IMission mission, bool success)
        {
            if (success)
                _logger.Log($"[MissionManager] Mission '{mission.Name}' completed successfully!");
            else
                _logger.Log($"[MissionManager] Mission '{mission.Name}' FAILED!");
        }
    }
}
