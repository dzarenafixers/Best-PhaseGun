// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace PhaseGun
{
    using Exiled.API.Interfaces;
    using PhaseGun.CustomItems;

    /// <inheritdoc />
    public class Config : IConfig
    {
        private IConfig _configImplementation;

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        public bool Debug
        {
            get => _configImplementation.Debug;
            set => _configImplementation.Debug = value;
        }

        /// <summary>
        /// Gets or sets a configurable instance of the <see cref="PhaseGunItem"/> class.
        /// </summary>
        public PhaseGunItem PhaseGun { get; set; } = new();
    }
}