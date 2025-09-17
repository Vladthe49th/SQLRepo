using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{

    public class Agent : IAgent
    {
        private static readonly Random _rnd = new();
        private readonly ILogger _logger;

        // Кодове ім'я агента (можна задати в конструкторі)
        public string Codename { get; private set; }

        // Загальне ім'я — повертає кодове (сумісність з фасадом/логами)
        public string Name => Codename;

        public Agent(string codename = "Agent 47", ILogger? logger = null)
        {
            Codename = codename ?? throw new ArgumentNullException(nameof(codename));
            _logger = logger ?? Logger.Instance;
        }

        /// <summary>
        /// Періодичне оновлення агента (викликається World.Simulate()).
        /// Можна розширити: патрулювання, зниження підозри тощо.
        /// </summary>
        public void Update()
        {
            // Наразі базова поведінка — просто логування "на місці".
            _logger.Log($"[Agent] {Name} scans the area.");
        }

        /// <summary>
        /// Спостереження за ціллю: логування і невелике підвищення підозри у цілі.
        /// </summary>
        public void Observe(ITarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (!target.IsAlive)
            {
                _logger.Log($"[Agent] {Name} tried to observe {target.Name} but target is already dead.");
                return;
            }

            // Невелика випадкова зміна підозри у цілі
            int delta = _rnd.Next(0, 3);
            target.SuspicionLevel = Math.Min(100, target.SuspicionLevel + delta);

            _logger.Log($"[Agent] {Name} observed {target.Name}. Suspicion -> {target.SuspicionLevel}");
        }

        /// <summary>
        /// Ліквідація цілі: делегує виконання самому Target через target.Eliminate()
        /// і логування в ILogger.
        /// </summary>
        public void Eliminate(ITarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (!target.IsAlive)
            {
                _logger.Log($"[Agent] {Name} attempted to eliminate {target.Name}, but target is already dead.");
                return;
            }

            target.Eliminate();
            _logger.Log($"[Agent] {Name} eliminated {target.Name}.");
        }

        public override string ToString() => $"{Name}";
    }



}
