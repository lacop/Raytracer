using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Raytracer.Math;
using Raytracer.Primitives;

namespace Raytracer.Acceleration
{
    class KDTreeAccelerator : AccelerationStructure
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO: getters/setters
        public int MaxDepth = 8;
        // TODO: minprims in node to terminate
        // 0.0 - 1.0, relative cost
        // TODO: find best values
        public float TraversalCost = 1.0f;
        public float IntersectionCost = 0.8f;
        public float EmptyBonus = 0.05f;

        private KDTreeNode root;

        private int stats_total = 0;
        private int stats_nonleaf = 0;
        private int stats_empty = 0;

        // Completely non-optimized ...
        // TODO: presorted scene, support for planes, ...
        public override void Build (List<Primitive> primitives)
        {
            log.InfoFormat(
                "Building KDTree for {0} primitives. Maxdepth: {1}, TraversalCost: {2}, IntersectionCost: {3}, EmptyBonus: {4}",
                primitives.Count, MaxDepth, TraversalCost, IntersectionCost, EmptyBonus);
            
            root = new KDTreeNode(Primitive.GetBoundingBox(primitives), primitives);
            
            Stopwatch sw = Stopwatch.StartNew();
            Subdivide(root, 0);
            sw.Stop();

            //TODO: build stats - time, # of (leaf) nodes, min/max prims in leaves, ...
            log.InfoFormat("Done building, took {0} ms, total {1} nodes, {2} leaf, {3} empty", sw.ElapsedMilliseconds, stats_total, stats_total-stats_nonleaf, stats_empty);

        }
        //TODO: move spliting/cost calc into node class
        private void Subdivide (KDTreeNode node, int depth)
        {
            stats_total++;

            if (depth > MaxDepth)
                return;

            if (node.primitives == null || node.primitives.Count == 0)
            {
                stats_empty++;
                return;
            }

            // Select biggest axis
            // TODO: really a good idea?
            int axis = 0;
            if ((node.bounds[1] > node.bounds[0]) && (node.bounds[1] > node.bounds[2]))
                axis = 1;
            if ((node.bounds[2] > node.bounds[0]) && (node.bounds[2] > node.bounds[1]))
                axis = 2;

            float currentCost = TraversalCost + node.bounds.GetSurfaceArea()*node.primitives.Count*IntersectionCost;

            SplitPosition[] splitPositions = new SplitPosition[node.primitives.Count*2];
            int i = 0;
            foreach (Primitive primitive in node.primitives)
            {
                splitPositions[i] = new SplitPosition(primitive.GetMinExtreme(axis), primitive, SplitPosition.BoundType.Begin);
                splitPositions[i+1] = new SplitPosition(primitive.GetMaxExtreme(axis), primitive, SplitPosition.BoundType.End);
                i += 2;
            }
            Array.Sort(splitPositions);

            int numLeft = 0;
            int numRight = node.primitives.Count;
            int bestPos = 0;
            float bestCost = float.PositiveInfinity;
            for (i = 0; i < splitPositions.Length; i++)
            {
                if (splitPositions[i].type == SplitPosition.BoundType.End) numRight--;

                AABB[] split = node.bounds.Split(axis, splitPositions[i].pos);

                // TODO: empty bonus
                float bonus = (numLeft == 0 || numRight == 0) ? (1.0f - EmptyBonus) : 1.0f;
                float newCost = 2*TraversalCost + bonus*IntersectionCost*(numLeft*split[0].GetSurfaceArea() + numRight*split[1].GetSurfaceArea());
                if (newCost < bestCost)
                {
                    bestCost = newCost;
                    bestPos = i;
                }

                if (splitPositions[i].type == SplitPosition.BoundType.Begin) numLeft++;
            }

            if (bestCost >= currentCost)
            {
                return;
            }

            AABB[] bestSplit = node.bounds.Split(axis, splitPositions[bestPos].pos);
            KDTreeNode bestLeft = new KDTreeNode(bestSplit[0], new List<Primitive>());
            KDTreeNode bestRight = new KDTreeNode(bestSplit[1], new List<Primitive>());

            for (i = 0; i < bestPos; i++)
            {
                if (splitPositions[i].type == SplitPosition.BoundType.Begin)
                    bestLeft.primitives.Add(splitPositions[i].primitive);
            }
            for (i = bestPos+1; i < splitPositions.Length; i++)
            {
                if (splitPositions[i].type == SplitPosition.BoundType.End)
                    bestRight.primitives.Add(splitPositions[i].primitive);
            }

            node.left = bestLeft;
            node.right = bestRight;

            Subdivide(bestLeft, depth+1);
            Subdivide(bestRight, depth+1);
            
        }

        public override bool FindIntersection (Ray r, out float closest, out Primitive closestPrimitive)
        {
            // TODO: improve tree traversal

            closest = float.PositiveInfinity;
            closestPrimitive = null;

            float t = 0;

            Queue<KDTreeNode> nodes = new Queue<KDTreeNode>();
            nodes.Enqueue(root);

            while (nodes.Count > 0)
            {
                KDTreeNode node = nodes.Dequeue();
                
                if (!node.bounds.Intersects(r))
                    continue;
                
                if (node.left == null || node.right == null) // Leaf
                {
                    foreach (Primitive primitive in node.primitives)
                    {
                        if (primitive.Intersects(r, out t) && t > 0)
                        {
                            if (t < closest)
                            {
                                closest = t;
                                closestPrimitive = primitive;
                            }
                        }
                    }
                }
                else
                {
                    nodes.Enqueue(node.left);
                    nodes.Enqueue(node.right);
                }

            }

            return (closestPrimitive != null);
        }

        private class KDTreeNode
        {
            public AABB bounds;
            public List<Primitive> primitives;
            public KDTreeNode left;
            public KDTreeNode right;

            public KDTreeNode (AABB bounds, List<Primitive> primitives)
            {
                this.bounds = bounds;
                this.primitives = primitives;
                left = null;
                right = null;
            }
        }

        private class SplitPosition : IComparable
        {
            public float pos;
            public Primitive primitive;
            public BoundType type;


            public SplitPosition(float pos, Primitive primitive, BoundType type)
            {
                this.pos = pos;
                this.primitive = primitive;
                this.type = type;
            }

            public int CompareTo(object obj)
            {
                return pos.CompareTo(((SplitPosition)obj).pos);
            }

            internal enum BoundType
            {
                Begin,
                End
            }

        }

    }

}
