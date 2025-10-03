using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public class ExecuteAction : IGameAction
    {
        public ActionType Type => ActionType.Eliminate;

        private readonly ITarget _target;
        private readonly bool _silent; 

        public ExecuteAction(ITarget target, bool silent = true)
        {
            _target = target;
            _silent = silent;
        }

        public void Execute(IAgent agent, IWorld? world = null)
        {
            if (!_target.IsAlive)
                return;

            

            _target.Eliminate();

            Logger.Instance.Log($"[EliminateAction] {agent.Name} eliminated {_target.Name} ({(_silent ? "silent" : "loud")})");

            if (world != null && !_silent)
            {
                _target.SuspicionLevel += 5;
                Logger.Instance.Log($"[EliminateAction] {_target.Name} caused loud elimination! Suspicion now {_target.SuspicionLevel}");
            }
        }

    }


}
