// -----------------------------------------------------------------------
// <copyright file="NHConfigurationException.cs" company="InitVent Consulting Services.">
//     Copyright (c) InitVent Consulting Services. All rights reserved.
// </copyright>
// <author>S.M. Saiful Islam</author>
// -----------------------------------------------------------------------

namespace InitVent.ORM
{
    using System;

    /// <summary>
    /// Any missing configuration value would trigger this exception.
    /// </summary>
    [Serializable]
    public class NHConfigurationException : Exception
    {
        public NHConfigurationException(string message, Exception innerException) : base (message, innerException) { }
    }
}