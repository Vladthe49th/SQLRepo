using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public interface IEliminationStrategy
    {
        void Eliminate(IAgent agent, ITarget target);
    }

    public class SilentKill : IEliminationStrategy
    {
        public void Eliminate(IAgent agent, ITarget target)
        {
            agent.Eliminate(target);
            Logger.Instance.Log($"{agent.Codename} silently eliminated {target.Name}");
        }
    }

    public class LoudKill : IEliminationStrategy
    {
        public void Eliminate(IAgent agent, ITarget target)
        {
            agent.Eliminate(target);
            Logger.Instance.Log($"{agent.Codename} loudly eliminated {target.Name} (bodies everywhere!)");
        }
    }
}

public class ExecuteAction : IGameAction
{
    public ActionType Type => ActionType.Eliminate;
    private readonly ITarget _target;
    private readonly IEliminationStrategy _strategy;
    private ITarget target2;
    private SilentKill silentKill;

    public ExecuteAction(ITarget target, IEliminationStrategy strategy)
    {
        _target = target;
        _strategy = strategy;
    }

    public ExecuteAction(ITarget target2, SilentKill silentKill)
    {
        this.target2 = target2;
        this.silentKill = silentKill;
    }

    public void Execute(IAgent agent, ITarget? target = null)
    {
        _strategy.Eliminate(agent, _target);
    }

    public void Execute(IAgent agent, IWorld? world = null)
    {
        throw new NotImplementedException();
    }
}


