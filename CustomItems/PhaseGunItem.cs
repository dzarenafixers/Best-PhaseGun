// -----------------------------------------------------------------------
// <copyright file="PhaseGunItem.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.Events.EventArgs.Player;

namespace PhaseGun.CustomItems
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using PhaseGun.Models;
    using UnityEngine;

    /// <inheritdoc />
    public class PhaseGunItem : CustomWeapon
    {
        private VoidManager voidManager;

        /// <inheritdoc />
        public override bool ShouldMessageOnGban => false;

        /// <inheritdoc />
        public override uint Id { get; set; } = 63;

        /// <inheritdoc />
        public override string Name { get; set; } = "Phase Gun";

        /// <inheritdoc />
        public override string Description { get; set; }

        /// <inheritdoc />
        public override float Weight { get; set; }

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new();

        /// <inheritdoc />
        public override ItemType Type { get; set; } = ItemType.GunCOM18;

        /// <inheritdoc />
        public override float Damage { get; set; } = 0f;

        /// <inheritdoc />
        public override byte ClipSize { get; set; } = 6;

        /// <summary>
        /// Gets or sets a value indicating whether the phase gun should affect tutorials.
        /// </summary>
        [Description("Whether the phase gun should affect tutorials.")]
        public bool AffectTutorials { get; set; } = false;

        /// <summary>
        /// Gets or sets the amount of time a hit player will be sent to the void.
        /// </summary>
        [Description("The amount of time a hit player will be sent to the void.")]
        public float VoidDuration { get; set; } = 6f;

        /// <summary>
        /// Gets or sets the effects to apply to players when they are sent to the void.
        /// </summary>
        [Description("The effects to apply to players when they are sent to the void.")]
        public List<ConfiguredEffect> Effects { get; set; } = new()
        {
            new ConfiguredEffect(EffectType.MovementBoost, byte.MaxValue),
        };

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            voidManager = new VoidManager(this);
            voidManager.Subscribe();
            base.SubscribeEvents();
        }

        /// <inheritdoc />
        protected override void UnsubscribeEvents()
        {
            voidManager.Unsubscribe();
            voidManager = null;
            base.UnsubscribeEvents();
        }

        /// <inheritdoc />
        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.IsAllowed = false;
            if (ev.Player.CurrentItem is not Firearm firearm)
                return;

            firearm.AmmoDrain--;
            if (Math.Abs(ev.Player.CameraTransform.rotation.eulerAngles.x - 88f) < 1f)
            {
                HandleShot(ev.Player);
                return;
            }

            object player;
            player = typeof(Player);
            if (ev.Direction != Vector3.zero && ev.ClaimedTarget != null)
            HandleShot(ev.ClaimedTarget);
        }

        /// <inheritdoc />
        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        private void HandleShot(Player target)
        {
            if (target.IsTutorial && !AffectTutorials)
                return;

            if (voidManager.CurrentlyVoiding.Contains(target))
                return;

            voidManager.Start(target);
        }
    }
}