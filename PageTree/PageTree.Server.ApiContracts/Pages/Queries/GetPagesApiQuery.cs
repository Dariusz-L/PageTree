using Corelibs.Basic.Net;

namespace PageTree.Server.ApiContracts
{
    public class GetPagesApiQuery
    {
        [FromRoute]
        public string Name { get; set; }
    }
}
