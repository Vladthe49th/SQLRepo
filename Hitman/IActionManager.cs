using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public interface IActionManager
    {
        // Add action to sequence
        void AddAction(IGameAction action);

        // Execute agent actions
        void Execute(IAgent agent, IWorld world);

        // Execute actions for all agents
        void Execute();

        // Execute all actions
        void ExecuteAll(IAgent agent);

        
        void Clear();
    }

}
