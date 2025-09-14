using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public interface ITargetManager
    {
        void AddTarget(ITarget target);
        IEnumerable<ITarget> GetAliveTargets();
        ITarget? GetRandomTarget();
    }

}
