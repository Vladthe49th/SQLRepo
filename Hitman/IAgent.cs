using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{

    public interface IAgent
    {
    
        string Name { get; }

        void Update();


        string Codename { get; }

        void Observe(ITarget target);
        void Eliminate(ITarget target);
    }

}
