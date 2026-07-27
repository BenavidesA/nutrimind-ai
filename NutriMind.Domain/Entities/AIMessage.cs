using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class AIMessage : BaseEntity
    {
        public string Role { get; set; } // "User" or "AI"
        public string Content { get; set; }

        public Guid AIConversationId { get; set; }
        public virtual AIConversation AIConversation { get; set; }
    }
}
