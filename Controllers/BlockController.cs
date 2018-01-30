using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TinyChain;
using apitest.ViewModels;

namespace apitest.Controllers
{   
    [Produces("application/json")]
	[Route("TinyChain")]
    public class BlockController : Controller
    {       
        private BlockChain _tinyChain;
        public BlockController(BlockChain tinyChain){

            _tinyChain = tinyChain;

        }

        [HttpGet("blocks")]
        public List<Block> blocks()
        {
            return _tinyChain.getBlockChain();
            Console.WriteLine("Served BlockChain");
        }

        [HttpPost]
        [Route("post")]
        public void postFunction ([FromBody]BlockViewModel blockModel){
            var model = blockModel;
            _tinyChain.generateNextBlock(model.data);
        }
    }
}
