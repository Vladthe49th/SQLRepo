using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public class ObserveAction : IGameAction
    {
        public ActionType Type => ActionType.Observe;

        private readonly ITarget _target;
        private ITarget target1;

        public ObserveAction(ITarget target)
        {
            _target = target;
        }

      

        public void Execute(IAgent agent, IWorld? world = null)
        {
            if (!_target.IsAlive)
                return;

           // Agent observing
            agent.Observe(_target);

         // Raising suspicion
            _target.SuspicionLevel += 1;

            Logger.Instance.Log($"[ObserveAction] {agent.Name} observed {_target.Name}. Suspicion now {_target.SuspicionLevel}");
        }


    }
}
