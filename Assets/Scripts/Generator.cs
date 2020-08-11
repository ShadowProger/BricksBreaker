using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

namespace Manybits
{
    public enum BlockType { None, Brick, DoubleBrick, BallBonus, StarBonus, ShieldBonus, DoubleBallBonus, Lazer }



    public class Generator
    {
        List<Rule> rules = new List<Rule>();
        GameManager gameManager;



        public Generator(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }



        public void AddRule(Rule rule)
        {
            rules.Add(rule);
        }



        public void Clear()
        {
            rules.Clear();
        }



        public List<BlockType> GetLine(int lineNumber, int maxWidth)
        {
            Rule rule = null;
            for (int i = 0; i < rules.Count; i++)
            {
                int min = rules[i].minLine == -1 ? 0 : rules[i].minLine;
                int max = rules[i].maxLine == -1 ? int.MaxValue : rules[i].maxLine;
                if (lineNumber >= min && lineNumber <= max)
                {
                    rule = rules[i];
                    break;
                }
            }

            if (rule == null)
                return null;

            List<BlockType> line = new List<BlockType>();

            int maxCount = maxWidth;

            for (int i = 0; i < rule.blocks.Count; i++)
            {
                bool isLuck = false;
                if (rule.blocks[i].chance == 100)
                {
                    isLuck = true;
                }
                else
                {
                    int chance = UnityEngine.Random.Range(0, 100);
                    if (chance <= rule.blocks[i].chance)
                        isLuck = true;
                }

                if (isLuck)
                {
                    int count = UnityEngine.Random.Range(rule.blocks[i].minCount, rule.blocks[i].maxCount + 1);

                    count = Mathf.Min(count, maxCount);

                    if (count > 0)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            BlockType blockType = rule.blocks[i].blockTypes.GetRandom();
                            if (blockType == BlockType.Brick && rule.doubleBlockChance > 0)
                            {
                                int chance = UnityEngine.Random.Range(0, 100);
                                if (chance <= rule.doubleBlockChance)
                                    blockType = BlockType.DoubleBrick;
                            }
                            line.Add(blockType);
                        }
                    }

                    maxCount -= count;
                }
            }

            if (maxCount > 0)
            {
                for (int i = 0; i < maxCount; i++)
                    line.Add(BlockType.None);
            }

            line.Shuffle();

            return line;
        }



        public void GenerateRules(int maxWidth)
        {
            Clear();

            Rule rule = new Rule();
            rule.minLine = -1;
            rule.maxLine = -1;
            rule.doubleBlockChance = 15;

            BlockChance block = new BlockChance();
            block.minCount = 1;
            block.maxCount = 1;
            block.chance = 100;
            block.blockTypes.Add(BlockType.BallBonus);

            rule.blocks.Add(block);

            block = new BlockChance();
            block.minCount = Random.Range(1, 3);
            block.maxCount = Random.Range(3, 5);
            block.chance = 100;
            block.blockTypes.Add(BlockType.Brick);

            rule.blocks.Add(block);

            block = new BlockChance();
            block.minCount = 1;
            block.maxCount = 1;
            block.chance = Random.Range(0, 21);
            block.blockTypes.Add(BlockType.ShieldBonus);
            block.blockTypes.Add(BlockType.DoubleBallBonus);
            block.blockTypes.Add(BlockType.Lazer);

            rule.blocks.Add(block);

            block = new BlockChance();
            block.minCount = 2;
            block.maxCount = maxWidth;
            block.chance = Random.Range(0, 71);
            block.blockTypes.Add(BlockType.Brick);

            rule.blocks.Add(block);

            AddRule(rule);
        }



        public List<List<BlockType>> GetLines(int startLine, int count, int maxWidth)
        {
            List<List<BlockType>> lines = new List<List<BlockType>>();
            for (int i = startLine; i < startLine + count; i++)
            {
                List<BlockType> line = GetLine(i, maxWidth);
                lines.Add(line);
            }

            return lines;
        }



        public bool Load(string fileName)
        {
            Debug.Log("[Generator] Load");

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Resources.Load(fileName).ToString());

            XmlNodeList dataList = xml.GetElementsByTagName("Rule");
            foreach (XmlNode ruleNode in dataList)
            {
                Rule rule = new Rule();
                rule.minLine = int.Parse(ruleNode.Attributes["minLine"].Value);
                rule.maxLine = int.Parse(ruleNode.Attributes["maxLine"].Value);
                rule.doubleBlockChance = int.Parse(ruleNode.Attributes["doubleBlockChance"].Value);

                XmlNodeList blocks = ruleNode.SelectNodes("Block");
                foreach (XmlNode blockNode in blocks)
                {
                    BlockChance block = new BlockChance();
                    block.minCount = int.Parse(blockNode.Attributes["minCount"].Value);
                    block.maxCount = int.Parse(blockNode.Attributes["maxCount"].Value);
                    block.chance = int.Parse(blockNode.Attributes["chance"].Value);

                    XmlNodeList blockTypes = blockNode.SelectNodes("BlockType");
                    foreach (XmlNode blockTypeNode in blockTypes)
                    {
                        BlockType blockType = BlockType.None;

                        switch (blockTypeNode.Attributes["name"].Value)
                        {
                            case "Brick":
                                blockType = BlockType.Brick;
                                break;
                            case "BallBonus":
                                blockType = BlockType.BallBonus;
                                break;
                            case "StarBonus":
                                blockType = BlockType.StarBonus;
                                break;
                            case "ShieldBonus":
                                blockType = BlockType.ShieldBonus;
                                break;
                            case "DoubleBallBonus":
                                blockType = BlockType.DoubleBallBonus;
                                break;
                            case "Lazer":
                                blockType = BlockType.Lazer;
                                break;
                        }
                        block.blockTypes.Add(blockType);
                    }
                    rule.blocks.Add(block);
                }
                rules.Add(rule);
            }

            Debug.Log("[Generator] Load is successful");
            return true;
        }
    }



    public class Rule
    {
        public int minLine;
        public int maxLine;
        public int doubleBlockChance;
        public List<BlockChance> blocks = new List<BlockChance>();
    }



    public class BlockChance
    {
        public List<BlockType> blockTypes = new List<BlockType>();
        public int chance;
        public int minCount;
        public int maxCount;
    }
}
