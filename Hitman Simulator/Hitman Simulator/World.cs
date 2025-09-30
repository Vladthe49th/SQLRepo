using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public class World : IWorld
    {
        private readonly List<IAgent> _agents = new();
        private readonly List<ITarget> _targets = new();

        public IReadOnlyCollection<IAgent> Agents => _agents.AsReadOnly();
        public IReadOnlyCollection<ITarget> Targets => _targets.AsReadOnly();

        IReadOnlyList<ITarget> IWorld.Targets => throw new NotImplementedException();

        public event Action<ITarget>? OnTargetMoved;

        public void AddAgent(IAgent agent) => _agents.Add(agent);

        public void AddTarget(ITarget target) => _targets.Add(target);

        public void Simulate()
        {
            throw new NotImplementedException();
        }
    }


}
