using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.DomainCommons.Models;

namespace WYYMusic.Domain.Entity
{
    public class BaseEntity : ISoftDelete, IHasCreationTime, IHasDeletionTime
    { 
        public long Id { get;protected set; }

        public bool IsDeleted {get;private set;}
        public DateTime? DeletionTime { get;private set; }
        public DateTime CreationTime { get; private set; } = DateTime.Now;
        public void SoftDelete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.Now;
        } 
    }
}
