using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public class TargetManager : ITargetManager
    {
        private readonly List<ITarget> _targets = new List<ITarget>();

        public void AddTarget(ITarget target) => _targets.Add(target);

        public IEnumerable<ITarget> GetAliveTargets() => _targets.Where(t => t.IsAlive);

        public ITarget? GetRandomTarget()
        {
            var alive = _targets.Where(t => t.IsAlive).ToList();
            if (!alive.Any()) return null;

            return alive[new Random().Next(alive.Count)];
        }
    }


}
