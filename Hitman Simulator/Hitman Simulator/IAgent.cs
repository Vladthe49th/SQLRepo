using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public interface IAgent
    {
        string Codename { get; }

        void Observe(ITarget target);
        void Eliminate(ITarget target);
    }

}
