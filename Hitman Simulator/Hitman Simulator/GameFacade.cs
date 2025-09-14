using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public class GameFacade : IGameFacade
    {
        private readonly IWorld _world;
        private readonly IActionManager _actionManager;
        private readonly MissionManager _missionManager;
        private readonly ILogger _logger;
        private readonly System.Collections.Generic.List<IAgent> _agents = new();

        public GameFacade(IWorld world, IActionManager actionManager)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _actionManager = actionManager ?? throw new ArgumentNullException(nameof(actionManager));
            _logger = Logger.Instance; 
            _missionManager = new MissionManager(_logger);
        }

        public void Initialize()
        {
            _logger.Log("[GameFacade] Initialized.");
            var defaults = MissionPack.GetDefaultMissions();
            foreach (var m in defaults) _missionManager.AddMission(m);
        }



        public void AddAgent(IAgent agent)
        {
            if (agent == null) return;
            _agents.Add(agent);
            _logger.Log($"[GameFacade] Agent {agent.Name} added.");
        }

        public void AddTargetToWorld(ITarget target)
        {
            if (target == null) return;
            bool exists = _world.Targets.Any(t => (t.Id != 0 && t.Id == target.Id) || (t.Id == 0 && t.Name == target.Name));
            if (!exists)
            {
                _world.AddTarget(target);
                _logger.Log($"[GameFacade] Target {target.Name} added to world.");
            }
            else
            {
                _logger.Log($"[GameFacade] Target {target.Name} already in world — skip add.");
            }
        }

        public void AddMission(IMission mission)
        {
            if (mission == null) return;
            _missionManager.AddMission(mission);
        }

        
        public void RunCampaign(int stepsPerMission = 5, int stepDelayMs = 1000)
        {
            _logger.Log("[GameFacade] Running campaign...");

            foreach (var mission in _missionManager.Missions)
            {
                _logger.Log($"\n=== Starting mission: {mission.Name} ===");
                _logger.Log(mission.Description);

                // 1) Adding targets to world
                foreach (var t in mission.Targets)
                {
                    AddTargetToWorld(t);
                }

                // 2) Generating basing actions for agents
                foreach (var target in mission.Targets)
                {
                    foreach (var agent in _agents)
                    {
                        
                        _actionManager.AddAction(new ObserveAction(target));
                        _actionManager.AddAction(new WaitAction(500)); 
                        
                        _actionManager.AddAction(new ExecuteAction(target, silent: true));
                    }
                }

                // 3) Lettin g ActionManager to perform a sequence
                for (int step = 0; step < stepsPerMission; step++)
                {
                    _logger.Log($"[GameFacade] Mission '{mission.Name}' — Step {step + 1}/{stepsPerMission}");
                    // Target movement
                    _world.Simulate();

                    // Actions for each agent
                    foreach (var agent in _agents)
                    {
                        _actionManager.Execute(agent, _world);
                    }

                    
                    Thread.Sleep(stepDelayMs);
                }

                // 4) Additional time for asynchronous tasks
                Thread.Sleep(500);

                // 5) Checking requisites for completion
                bool success = false;
                foreach (var agent in _agents)
                {
                    if (mission.CheckCompletion(_world, agent))
                    {
                        success = true;
                        break;
                    }
                }

                _missionManager.LogMissionResult(mission, success);
            }

            _logger.Log("[GameFacade] Campaign finished.");
        }
    }

}
