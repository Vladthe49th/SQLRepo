using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public interface IMission
    {
        string Name { get; }
        string Description { get; }
        List<ITarget> Targets { get; }
        bool IsCompleted { get; }
        bool CheckCompletion(IWorld world, IAgent agent);
    }

    public class Mission : IMission
    {
        public string Name { get; }
        public string Description { get; }
        public List<ITarget> Targets { get; }
        public bool IsCompleted { get; private set; }

        private readonly Func<IWorld, IAgent, bool> _completionCondition;

        public Mission(string name, string description, List<ITarget> targets, Func<IWorld, IAgent, bool> completionCondition)
        {
            Name = name;
            Description = description;
            Targets = targets;
            _completionCondition = completionCondition;
            IsCompleted = false;
        }

        public bool CheckCompletion(IWorld world, IAgent agent)
        {
            IsCompleted = _completionCondition(world, agent);
            return IsCompleted;
        }
    }
}
