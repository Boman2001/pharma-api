using Core.Domain.Models;
using WebApi.Models.IcpcCodes;

namespace WebApi.Models.Episodes
{
    public class EpisodeDto :  BaseEpisodeDto
    {
        public int Id { get; set; }
        public IcpcCodeDto IcpcCode { get; set; }
    }
}