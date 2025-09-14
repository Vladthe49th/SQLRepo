using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public class ActionManager : IActionManager
    {
        private readonly Queue<IGameAction> _actions = new();
        private readonly IThreadPoolManager? _threadPool;
        private readonly ILogger? _logger;
        private readonly IWorld? _world;
        private readonly object _lock = new();

        
        public ActionManager(IThreadPoolManager threadPool, ILogger logger)
        {
            _threadPool = threadPool ?? throw new ArgumentNullException(nameof(threadPool));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

       
        public void AddAction(IGameAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (_threadPool == null || _logger == null)
                throw new InvalidOperationException("ActionManager not configured for queued actions. Construct with (IThreadPoolManager, ILogger).");

            lock (_lock)
            {
                _actions.Enqueue(action);
            }

            _logger.Log($"[ActionManager] Enqueued action: {action.Type}");
        }

        // Execute actions for a specific agent
        public void Execute(IAgent agent, IWorld world)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (world == null) throw new ArgumentNullException(nameof(world));

           
            if (_threadPool != null && _logger != null)
            {
                while (true)
                {
                    IGameAction? action = null;
                    lock (_lock)
                    {
                        if (_actions.Count == 0) break;
                        action = _actions.Dequeue();
                    }

                    if (action == null) continue;

                    var captured = action;
                    _threadPool.QueueAction(() =>
                    {
                        try
                        {
                            captured.Execute(agent, world);
                            _logger.Log($"[ActionManager] {agent.Name} performed {captured.Type}");
                        }
                        catch (Exception ex)
                        {
                            _logger.Log($"[ActionManager] Error during action execution: {ex.Message}");
                        }
                    });
                }
            }
            else
            {
                // Simple world-driven logic 
                foreach (var target in world.Targets)
                {
                    if (!target.IsAlive) continue;

                    // Agent observes, and eliminates if the suspicion level is high
                    agent.Observe(target);

                    if (target.SuspicionLevel > 15)
                    {
                        agent.Eliminate(target);
                        Logger.Instance.Log($"{agent.Name} eliminated {target.Name} due to high suspicion.");
                    }
                }
            }
        }

     
        public void Execute()
        {
            if (_world == null) throw new InvalidOperationException("ActionManager: no world assigned for parameterless Execute().");
            foreach (var agent in _world.Agents)
            {
                Execute(agent, _world);
            }
        }

        // Execute all actions for an agent — wrapper
        public void ExecuteAll(IAgent agent)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (_world == null) throw new InvalidOperationException("ActionManager: no world assigned for ExecuteAll.");
            Execute(agent, _world);
        }

        
        public void Clear()
        {
            lock (_lock)
            {
                _actions.Clear();
            }

            (_logger ?? Logger.Instance).Log("[ActionManager] Queue cleared.");
        }
    }


}
