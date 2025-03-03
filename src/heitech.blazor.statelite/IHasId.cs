﻿using System;

namespace heitech.blazor.statelite
{
    /// <summary>
    /// Defines the ids as guids for the store
    /// </summary>
    public interface IHasId<T>
    {
        T Id { get; }
    }
}