using NutriMind.Domain.Common;
using System;
using System.Collections.Generic;

namespace NutriMind.Domain.Entities
{
    public class AIConversation : BaseEntity
    {
        public string Title { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<AIMessage> AIMessages { get; set; } = new List<AIMessage>();
    }
}
