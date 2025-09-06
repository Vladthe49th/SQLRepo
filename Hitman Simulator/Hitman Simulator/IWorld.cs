using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public interface IWorld
    {
        IReadOnlyList<ITarget> Targets { get; }

        void AddTarget(ITarget target);
        void Simulate();
    }

}
