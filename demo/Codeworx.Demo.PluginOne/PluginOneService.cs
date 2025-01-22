using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Demo.PluginOne.Contract;
using Codeworx.Rest;

namespace Codeworx.Demo.PluginOne
{
    public class PluginOneService : IPluginOneService
    {
        private static readonly Dictionary<Guid, PluginOneObject> _items;

        static PluginOneService()
        {
            _items = new Dictionary<Guid, PluginOneObject>
            {
                { Guid.Parse("00000000-0000-0000-0000-000000000001"), new PluginOneObject { Value1 = "abcdefg", Value2 = 123m } },
                { Guid.Parse("00000000-0000-0000-0000-000000000002"), new PluginOneObject { Value1 = "gfedcba", Value2 = 321m } },
            };
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PluginOneObject>> GetAsync()
        {
            return Task.FromResult<IEnumerable<PluginOneObject>>(_items.Values);
        }

        public Task<PluginOneObject> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PluginOneObject> PostAsync([BodyMember] PluginOneObject content)
        {
            throw new NotImplementedException();
        }

        public Task<PluginOneObject> PutAsync(Guid id, [BodyMember] PluginOneObject content)
        {
            throw new NotImplementedException();
        }
    }
}