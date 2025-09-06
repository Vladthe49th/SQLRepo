using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public class TargetManager
    {
        private readonly List<Target> _targets = new List<Target>();

        public void AddTarget(Target target) => _targets.Add(target);

        public IEnumerable<Target> GetAliveTargets() => _targets.Where(t => t.IsAlive);

        public Target? GetRandomTarget()
        {
            return _targets.Where(t => t.IsAlive)
                           .OrderBy(_ => Guid.NewGuid())
                           .FirstOrDefault();
        }
    }

}
