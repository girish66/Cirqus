﻿using System.Collections.Generic;

namespace d60.EventSorcerer.Events
{
    /// <summary>
    /// Metadata collection that stores a bunch of key-value pairs that can be used for
    /// cross-cutting concerns like e.g. handling multi-tenancy, auditing, etc.
    /// </summary>
    public class Metadata : Dictionary<string, object>
    {
    }
}