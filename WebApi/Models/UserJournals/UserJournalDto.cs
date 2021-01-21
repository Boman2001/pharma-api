using System;
using WebApi.Models.IcpcCodes;

namespace WebApi.Models.UserJournals
{
    public class UserJournalDto :  BaseUserJournalDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}