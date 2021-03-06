﻿using EventsApp.DataModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    public class EventUnitOfWork : IEventUnitOfWork
    {
        private EventContext context;
        public IEventRepository Events { get; set; }
        public IUserRepository Users { get; set; }
        public IInviteRepository Invites { get; set; }
        public IInviteLinkRepository InviteLinks { get; set; }

        public EventUnitOfWork()
        {
            context = new EventContext();
            Events = new EventRepository(context);
            Users = new UserRepository(context, new UserManager<AppUser, string>(new UserStore<AppUser>(context)));
            Invites = new InviteRepository(context);
            InviteLinks = new InviteLinkRepository(context);
        }

        public EventUnitOfWork(EventContext context)
        {
            this.context = context;
            Events = new EventRepository(context);
            Users = new UserRepository(context, new UserManager<AppUser, string>(new UserStore<AppUser>(context)));
            Invites = new InviteRepository(context);
            InviteLinks = new InviteLinkRepository(context);
        }

        public void Save()
        {
            context.SaveChanges();
            ContextStateHelper.ResetModificationState(context);
        }

  
    }
}
