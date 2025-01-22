using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Rest;

namespace Codeworx.Demo.PluginOne.Contract
{
    [RestRoute("api/plugin-one")]
    public interface IPluginOneService
    {
        [RestGet("{id}")]
        [ResponseType(404, typeof(NotFoundError))]
        [ResponseType(200, typeof(PluginOneObject))]
        Task<PluginOneObject> GetByIdAsync(Guid id);

        [RestGet]
        Task<IEnumerable<PluginOneObject>> GetAsync();

        [RestPost]
        Task<PluginOneObject> PostAsync([BodyMember] PluginOneObject content);

        [RestPut]
        [ResponseType(404, typeof(NotFoundError))]
        [ResponseType(200, typeof(PluginOneObject))]
        Task<PluginOneObject> PutAsync(Guid id, [BodyMember] PluginOneObject content);

        [RestDelete]
        [ResponseType(404, typeof(NotFoundError))]
        [ResponseType(200)]
        Task DeleteAsync(Guid id);
    }
}
