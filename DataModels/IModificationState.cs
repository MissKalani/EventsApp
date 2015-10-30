using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public enum ModificationState
    {
        Unchanged,
        Added,
        Modified,
        Deleted
    }

    public interface IModificationState
    {
        ModificationState ModificationState { get; set; }
    }
}
