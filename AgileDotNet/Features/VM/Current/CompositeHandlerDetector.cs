using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features.VM.Current
{
    public class CompositeHandlerDetector
    {
        private struct HandlerState
        {
            public readonly List<BlockSigInfo> BlockSigInfos;
            public readonly int BlockIndex;
            public int HashIndex;

            public HandlerState(List<BlockSigInfo> blockSigInfos, int blockIndex, int hashIndex)
            {
                BlockSigInfos = blockSigInfos;
                BlockIndex = blockIndex;
                HashIndex = hashIndex;
            }

            public HandlerState Clone() => new HandlerState(BlockSigInfos, BlockIndex, HashIndex);
        }
        private struct MatchState
        {
            public HandlerState OtherState;
            public HandlerState CompositeState;

            public MatchState(HandlerState OtherState, HandlerState CompositeState)
            {
                this.OtherState = OtherState;
                this.CompositeState = CompositeState;
            }
        }
        private struct FindHandlerState
        {
            public HandlerState CompositeState;
            public readonly Dictionary<int, bool> VisitedCompositeBlocks;
            public bool Done;

            public FindHandlerState(HandlerState compositeState)
            {
                CompositeState = compositeState;
                VisitedCompositeBlocks = new Dictionary<int, bool>();
                Done = true;
            }
            public FindHandlerState(HandlerState compositeState, Dictionary<int, bool> visitedCompositeBlocks, bool done)
            {
                CompositeState = compositeState;
                VisitedCompositeBlocks = visitedCompositeBlocks;
                Done = done;
            }

            public FindHandlerState Clone() => new FindHandlerState(CompositeState.Clone(), VisitedCompositeBlocks, Done);
        }

        private readonly List<MethodSigInfo> handlers;
        private Stack<MatchState> stack = new Stack<MatchState>();


        private static HandlerState? GetNextHandlerState(ref FindHandlerState findState)
        {
            for (int i = 0; i < findState.CompositeState.BlockSigInfos.Count; i++)
                if (findState.VisitedCompositeBlocks.ContainsKey(i))
                    continue;
                else return new HandlerState(findState.CompositeState.BlockSigInfos, i, 0);

            return null;
        }
        private static bool Compare(ref HandlerState handler, ref HandlerState composite)
        {
            List<BlockElementHash> hhashes = handler.BlockSigInfos[handler.BlockIndex].Hashes;
            int hIdx = handler.HashIndex;
            List<BlockElementHash> chashes = composite.BlockSigInfos[composite.BlockIndex].Hashes;
            int cIdx = composite.HashIndex;

            while (true)
            {
                if (hIdx >= hhashes.Count && cIdx >= chashes.Count)
                    break;

                if (hIdx >= hhashes.Count)
                    if (handler.BlockSigInfos[handler.BlockIndex].EndsInRet)
                        break;

                if (hIdx >= hhashes.Count || cIdx >= chashes.Count)
                    return false;

                BlockElementHash hhash = hhashes[hIdx++];
                BlockElementHash chash = chashes[cIdx++];

                if (chash != hhash)
                    return false;
            }

            handler.HashIndex = hIdx;
            composite.HashIndex = cIdx;

            return true;
        }
        private bool Matches(List<BlockSigInfo> handler, ref FindHandlerState findState)
        {
            HandlerState? nextState = null;

            stack.Clear();
            stack.Push(new MatchState(new HandlerState(handler, 0, 0), findState.CompositeState));

            while(stack.Count > 0)
            {
                MatchState mState = stack.Pop();

                if (mState.CompositeState.HashIndex == 0)
                {
                    if (findState.VisitedCompositeBlocks.ContainsKey(mState.CompositeState.BlockIndex))
                        continue;

                    findState.VisitedCompositeBlocks[mState.CompositeState.BlockIndex] = true;
                }
                else if (!findState.VisitedCompositeBlocks.ContainsKey(mState.CompositeState.BlockIndex))
                    throw new ApplicationException("Block was not visited");

                if (!Compare(ref mState.OtherState, ref mState.CompositeState))
                    return false;

                BlockSigInfo hblock = mState.OtherState.BlockSigInfos[mState.OtherState.BlockIndex];
                List<BlockElementHash> hhashes = hblock.Hashes;
                int hhashIdx = mState.OtherState.HashIndex;
                BlockSigInfo cblock = mState.CompositeState.BlockSigInfos[mState.CompositeState.BlockIndex];
                List<BlockElementHash> chashes = cblock.Hashes;
                int chashIdx = mState.CompositeState.HashIndex;

                if (hhashIdx < hhashes.Count)
                    return false;

                if(chashIdx < chashes.Count)
                {
                    if (hblock.Targets.Count != 0)
                        return false;

                    if (hblock.EndsInRet)
                    {
                        if (nextState != null)
                            return false;

                        nextState = mState.CompositeState;
                    }
                }
                else
                {
                    if (cblock.Targets.Count != hblock.Targets.Count)
                        return false;
                    if (cblock.HasFallThrough != hblock.HasFallThrough)
                        return false;

                    for(int i = 0; i < cblock.Targets.Count; i++)
                    {
                        HandlerState hs = new HandlerState(handler, hblock.Targets[i], 0);
                        HandlerState cs = new HandlerState(findState.CompositeState.BlockSigInfos, cblock.Targets[i], 0);
                        stack.Push(new MatchState(hs, cs));
                    }
                }
            }

            if (nextState == null && findState.VisitedCompositeBlocks.Count != findState.CompositeState.BlockSigInfos.Count)
                nextState = GetNextHandlerState(ref findState);

            if(nextState == null)
            {
                if (findState.VisitedCompositeBlocks.Count != findState.CompositeState.BlockSigInfos.Count)
                    return false;

                findState.Done = true;
                return true;
            }
            else
            {
                if (findState.CompositeState.BlockIndex == nextState.Value.BlockIndex && findState.CompositeState.HashIndex == nextState.Value.HashIndex)
                    return false;

                findState.CompositeState = nextState.Value;

                if (findState.CompositeState.HashIndex == 0)
                    findState.VisitedCompositeBlocks.Remove(findState.CompositeState.BlockIndex);

                return true;
            }
        }
        private MethodSigInfo FindHandlerMethod(ref FindHandlerState execState)
        {
            foreach (MethodSigInfo handler in handlers)
            {
                FindHandlerState execStateNew = execState.Clone();

                if (!Matches(handler.BlockSigInfos, ref execStateNew))
                    continue;

                execState = execStateNew;

                return handler;
            }

            return null;
        }


        public CompositeHandlerDetector(IList<MethodSigInfo> handlers)
        {
            this.handlers = new List<MethodSigInfo>(handlers);

            this.handlers.Sort((a, b) =>
            {
                int r = b.BlockSigInfos.Count.CompareTo(a.BlockSigInfos.Count);

                if (r != 0)
                    return r;

                return b.BlockSigInfos[0].Hashes.Count.CompareTo(a.BlockSigInfos[0].Hashes.Count);
            });
        }

        public bool FindHandlers(CompositeOpCodeHandler composite)
        {
            composite.TypeCodes.Clear();
            
            FindHandlerState execState = new FindHandlerState(new HandlerState(composite.BlockSigInfos, 0, 0));
            
            while (!execState.Done)
            {
                MethodSigInfo handler = FindHandlerMethod(ref execState);
                Utils.DebugN("handler", handler);
                if (handler == null)
                    return false;

                composite.TypeCodes.Add(handler.TypeCode);
            }

            return composite.TypeCodes.Count != 0;
        }
    }
}
