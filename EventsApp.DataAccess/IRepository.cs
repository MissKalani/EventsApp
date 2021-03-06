﻿using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    public interface IRepository<T> where T : IModificationState
    {
        /// <summary>
        /// Attach an entity graph. The modification state must be properly set for all nodes in the graph.
        /// </summary>
        void Attach(T entity);
    }
}
