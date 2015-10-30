using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public static class ContextStateHelper
    {
        private static EntityState GetEntityStateFromModificationState(ModificationState state)
        {
            switch (state)
            {
                case ModificationState.Unchanged: return EntityState.Unchanged;
                case ModificationState.Added: return EntityState.Added;
                case ModificationState.Modified: return EntityState.Modified;
                case ModificationState.Deleted: return EntityState.Deleted;
                default: throw new ArgumentException("No matching entity state for given modification state.");
            }
        }

        /// <summary>
        /// Updates the entity state to match the modification state. This can be used to appropriately set the
        /// entity state of entire graphs in disconnected applications.
        /// </summary>
        public static void ApplyStateChanges(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<IModificationState>())
            {
                IModificationState state = entry.Entity;
                entry.State = GetEntityStateFromModificationState(state.ModificationState);
            }
        }
    }
}
