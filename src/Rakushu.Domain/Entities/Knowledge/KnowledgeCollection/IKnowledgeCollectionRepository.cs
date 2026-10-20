using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;


public interface IKnowledgeCollectionRepository : IBaseRepository<KnowledgeCollection, KnowledgeCollectionId>
{
}
