using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public interface IWorld
    {
        IReadOnlyList<ITarget> Targets { get; }
        IReadOnlyCollection<IAgent> Agents { get; }
        event Action<ITarget>? OnTargetMoved;
        void AddTarget(ITarget target);
        void AddAgent(IAgent agent);
        void Simulate();
    }


}
