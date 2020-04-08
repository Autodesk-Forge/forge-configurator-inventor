﻿using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Abstract class for Forge App definition
    /// </summary>
    public abstract class ForgeAppConfigBase
    {
        public abstract int EngineVersion { get; }
        public string Engine => $"Autodesk.Inventor+{EngineVersion}";

        public abstract string Description { get; }
        public abstract string Id { get; }
        public abstract string Label { get; }

        public AppBundle Bundle
        {
            get
            {
                if (_appBundle == null)
                {
                    _appBundle = new AppBundle
                    {
                        Engine = Engine,
                        Id = Id,
                        Description = Description
                    };
                }
                return _appBundle;
            }
        }

        private AppBundle _appBundle;

        public string ActivityId => Id;
        public string ActivityLabel => Label;

        /// <summary>
        /// Command line for activity.
        /// </summary>
        public abstract List<string> ActivityCommandLine { get; }

        /// <summary>
        /// Activity parameters.
        /// </summary>
        public abstract Dictionary<string, Parameter> ActivityParams { get; }

        /// <summary>
        /// Arguments for workitem.
        /// </summary>
        public abstract Dictionary<string, IArgument> WorkItemArgs { get; }
    }
}