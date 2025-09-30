using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public class WaitAction : IGameAction
    {
        public ActionType Type => ActionType.Wait;

        private readonly int _milliseconds;

        public WaitAction(int milliseconds = 2000)
        {
            _milliseconds = milliseconds;
        }

        public void Execute(IAgent agent, IWorld? world = null)
        {
            Logger.Instance.Log($"[WaitAction] {agent.Name} is waiting for {_milliseconds} ms...");
            Thread.Sleep(_milliseconds); 
        }
    }


}
