using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Spell.Event;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Spell
{
    public delegate void CastMethodDelegate(Spell spell);

    public partial class Spell
    {
        [CastMethodHandler(CastMethod.Normal)]
        private void NormalHandler()
        {
            events.EnqueueEvent(new SpellEvent(parameters.SpellInfo.Entry.CastTime / 1000d, Execute)); // enqueue spell to be executed after cast time
        }

        [CastMethodHandler(CastMethod.Multiphase)]
        private void MultiphaseHandler()
        {
            status = SpellStatus.Executing;
            log.Trace($"Spell {parameters.SpellInfo.Entry.Id} has started executing.");

            uint spellDelay = 0;
            for(int i = 0; i < parameters.SpellInfo.Phases.Count; i++)
            {
                int index = i;
                SpellPhaseEntry spellPhase = parameters.SpellInfo.Phases[i];
                spellDelay += spellPhase.PhaseDelay;
                events.EnqueueEvent(new SpellEvent(spellDelay / 1000d, () =>
                {
                    currentPhase = (byte)spellPhase.OrderIndex;
                    Execute();

                    targets.ForEach(t => t.Effects.Clear());
                }));
            }
        }

        [CastMethodHandler(CastMethod.Channeled)]
        [CastMethodHandler(CastMethod.ChanneledField)]
        private void ChanneledHandler()
        {
            events.EnqueueEvent(new SpellEvent(parameters.SpellInfo.Entry.ChannelInitialDelay / 1000d, () =>
            {
                Execute();

                targets.ForEach(t => t.Effects.Clear());
            })); // Execute after initial delay
            events.EnqueueEvent(new SpellEvent(parameters.SpellInfo.Entry.ChannelMaxTime / 1000d, Finish)); // End Spell Cast

            uint numberOfPulses = (uint)MathF.Floor(parameters.SpellInfo.Entry.ChannelMaxTime / parameters.SpellInfo.Entry.ChannelPulseTime); // Calculate number of "ticks" in this spell cast

            // Add ticks at each pulse
            for (int i = 1; i <= numberOfPulses; i++)
                events.EnqueueEvent(new SpellEvent((parameters.SpellInfo.Entry.ChannelInitialDelay + (parameters.SpellInfo.Entry.ChannelPulseTime * i)) / 1000d, () =>
                {
                    Execute();

                    targets.ForEach(t => t.Effects.Clear());
                }));
        }

        [CastMethodHandler(CastMethod.ChargeRelease)]
        private void ChargeReleaseHandler()
        {
            if (parameters.ParentSpellInfo == null)
            {
                totalThresholdTimer = (uint)(parameters.SpellInfo.Entry.ThresholdTime / 1000d);

                // Keep track of cast time increments as we create timers to adjust thresholdValue
                uint nextCastTime = 0;
                
                // Create timers for each thresholdEntry's timer increment
                foreach (Spell4ThresholdsEntry thresholdsEntry in parameters.SpellInfo.Thresholds)
                {
                    nextCastTime += thresholdsEntry.ThresholdDuration;

                    if (thresholdsEntry.OrderIndex == 0)
                        continue;

                    events.EnqueueEvent(new SpellEvent(parameters.SpellInfo.Entry.CastTime / 1000d + nextCastTime / 1000d, () =>
                    {
                        thresholdValue = thresholdsEntry.OrderIndex;
                        SendThresholdUpdate();
                    }));
                }
            }

            events.EnqueueEvent(new SpellEvent(parameters.SpellInfo.Entry.CastTime / 1000d, Execute)); // enqueue spell to be executed after cast time
        }

        [CastMethodHandler(CastMethod.RapidTap)]
        private void RapidTapHandler()
        {
            if (parameters.ParentSpellInfo == null)
                events.EnqueueEvent(new SpellEvent(parameters.SpellInfo.Entry.CastTime / 1000d + parameters.SpellInfo.Entry.ThresholdTime / 1000d, Finish)); // enqueue spell to be executed after cast time

            events.EnqueueEvent(new SpellEvent(parameters.SpellInfo.Entry.CastTime / 1000d, Execute)); // enqueue spell to be executed after cast time
        }
    }
}
