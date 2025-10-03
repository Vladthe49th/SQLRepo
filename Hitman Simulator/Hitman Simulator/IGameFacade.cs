using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public interface IGameFacade
    {
        void Initialize();
        void AddAgent(IAgent agent);
        void AddTargetToWorld(ITarget target);
        void AddMission(IMission mission);
        void RunCampaign(int stepsPerMission = 5, int stepDelayMs = 1000);
    }

}
