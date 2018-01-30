using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace TinyChain
{
    public class BlockChain
    {
    	private List<Block> tinyChain;
    	public string aha;
    	public BlockChain()
		{
			Console.WriteLine("Blockchain initiated...");
			tinyChain = new List<Block>();
			tinyChain.Add(generateGenesis());
			aha = DateTime.Now.ToString("yyyyMMddHHmmss.fff");
			/*for (int i = 0; i < 10; i = i + 1) {
				Block nextBlock = newBlock(previousBlock);
				tinyChain.Add(nextBlock);
				previousBlock = nextBlock;
			}*/
			
			foreach(var block in tinyChain){
				Console.WriteLine("BlockNr: " + block.Index);
				Console.WriteLine("Hash: " + block.Hash);
				Console.WriteLine("Data: " + block.Data);
				Console.WriteLine(" ");
			}
		}
		public void generateNextBlock(string blockData){
			Block lastBlock = getLatestBlock();
			int nextIndex = lastBlock.Index + 1;
			string nextTimestamp = DateTime.Now.ToString("yyyyMMddHHmmss.fff");
			string nextPrevHash = lastBlock.Hash;
			Block newBlock = new Block(nextIndex, nextTimestamp, blockData, nextPrevHash);
			addBlock(newBlock);
			//broadcast latest
		}

		public static Block generateGenesis(){
			return new Block(0, "aSISDfasiofha2342sDHgfISDFLK2hs", "nr0data", "0");
		}

		public List<Block> getBlockChain(){
			return tinyChain;
		}

		public Block getLatestBlock(){
			return tinyChain[tinyChain.Count -1];
		}

		public void addBlock(Block newBlock){
			if(isValidNewBlock(newBlock, getLatestBlock())){
				tinyChain.Add(newBlock);
				Console.WriteLine("New block added to chain: " + newBlock);
			} else {
				Console.WriteLine("Block invalid. Did not add to chain");
			}
		}
		
		public bool isValidNewBlock(Block newBlock, Block previousBlock){
			if(newBlock.Data == null){
				Console.WriteLine("invalid Data");
				return false;
			} else if(previousBlock.Index + 1 != newBlock.Index){
				Console.WriteLine("invalid Index");
				return false;
			} else if (previousBlock.Hash != newBlock.Previous_hash){
				Console.WriteLine("invalid Previous Hash");
				return false;
			} else return true;
		}
		
	}

	public class Block
	{
		public int Index;
		public string Hash;
		public string Previous_hash;
		public string Timestamp;
		public string Data;
		

		public Block(int index, string timestamp, string data, string previous_hash){
			Index = index;
			Timestamp = timestamp;
			Data = data;
			Previous_hash = previous_hash;
			Hash = getHashSha256((Index + Timestamp + Data + Previous_hash).ToString());
		}

		private static string getHashSha256(string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			SHA256Managed hashstring = new SHA256Managed();
			byte[] hash = hashstring.ComputeHash(bytes);
			string hashString = string.Empty;
			foreach (byte x in hash)
			{
				hashString += String.Format("{0:x2}", x);
			}
			return hashString;
		}
	}
}