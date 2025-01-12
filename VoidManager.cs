// -----------------------------------------------------------------------
// <copyright file="VoidManager.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Doors;

namespace PhaseGun
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using Mirror;
    using PhaseGun.CustomItems;
    using PhaseGun.Models;
    using UnityEngine;

    /// <summary>
    /// Handles the void aspect of the <see cref="PhaseGunItem"/>.
    /// </summary>
    public class VoidManager
    {
        private readonly List<Player> currentlyVoidingValue = new();
        private readonly PhaseGunItem phaseGun;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoidManager"/> class.
        /// </summary>
        /// <param name="phaseGun">An instance of the <see cref="PhaseGunItem"/> class.</param>
        public VoidManager(PhaseGunItem phaseGun) => this.phaseGun = phaseGun;

        /// <summary>
        /// Gets the players that are currently considered to be in the void.
        /// </summary>
        public ReadOnlyCollection<Player> CurrentlyVoiding => currentlyVoidingValue.AsReadOnly();

        /// <summary>
        /// Subscribes to all required events.
        /// </summary>
        public void Subscribe()
        {
        }

        /// <summary>
        /// Unsubscribes from all required events.
        /// </summary>
        public void Unsubscribe()
        {
        }

        /// <summary>
        /// Starts the void sequence for the specified player.
        /// </summary>
        /// <param name="player">The player to start voiding.</param>
        public void Start(Player player)
        {
            currentlyVoidingValue.Add(player);
            player.DisableAllEffects();
            foreach (ConfiguredEffect effect in phaseGun.Effects)
                effect.Apply(player);

            ToggleDoors(player, true);

            Timing.CallDelayed(phaseGun.VoidDuration, () => Stop(player));
        }

        private static void ToggleDoors(Player player, bool isHidden)
        {
            bool failedSyncVar = false;
            foreach (Door door in Door.List)
            {
                DoorVariant doorVariant = door.Base;
                NetworkIdentity identity = doorVariant.netIdentity;
                doorVariant.transform.localScale = isHidden ? Vector3.zero : Vector3.one;

                ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage
                {
                    netId = identity.netId,
                };

                NetworkConnection playerCon = player.Connection;
                playerCon.Send(destroyMessage);

                object[] parameters = { identity, playerCon };
                typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);

                if (failedSyncVar)
                    continue;

                try
                {
                    if (doorVariant is BreakableDoor)
                        player.SendFakeSyncVar(identity, typeof(BreakableDoor), nameof(BreakableDoor.NetworkTargetState), isHidden || door.IsOpen);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to manipulate TargetState for void: " + e);
                    failedSyncVar = true;
                }
            }
        }

        private void Stop(Player player)
        {
            player.DisableAllEffects();
            ToggleDoors(player, false);
            currentlyVoidingValue.Remove(player);
        }
    }
}