// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace PhaseGun
{
    using System;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;

    /// <inheritdoc />
    public class Plugin : Plugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "MONCEF50G";

        /// <inheritdoc/>
        public override string Name => "PhaseGun";

        /// <inheritdoc/>
        public override string Prefix => "PhaseGun";

        /// <inheritdoc/>
        public override Version Version { get; } = new(1, 0, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new(9, 2, 1);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Config.PhaseGun?.Register();
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Config.PhaseGun?.Unregister();
            base.OnDisabled();
        }
    }
}