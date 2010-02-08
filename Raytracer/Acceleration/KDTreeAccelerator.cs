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
        // 0.0 - 1.0, relative cost
        public float TraversalCost = 1.0f;
        public float IntersectionCost = 0.6f;
        //TODO: reward for empty nodes

        private KDTreeNode root;

        // Completely non-optimized ...
        // TODO: presorted scene, support for planes, ...
        public override void Build (List<Primitive> primitives)
        {
            log.InfoFormat(
                "Building KDTree for {0} primitives. Maxdepth: {1}, TraversalCost: {2}, IntersectionCost: {3}",
                primitives.Count, MaxDepth, TraversalCost, IntersectionCost);
            
            root = new KDTreeNode(Primitive.GetBoundingBox(primitives), primitives);
            Subdivide(root, 0);

            //TODO: build stats - time, # of (leaf) nodes, min/max prims in leaves, ...
            log.InfoFormat("Done building");

        }
        //TODO: move spliting/cost calc into node class
        private void Subdivide (KDTreeNode node, int depth)
        {
            if (depth > MaxDepth)
                return;

            if (node.primitives == null)
                return;

            // Select biggest axis
            // TODO: really a good idea?
            int axis = 0;
            if ((node.bounds[1] > node.bounds[0]) && (node.bounds[1] > node.bounds[2]))
                axis = 1;
            if ((node.bounds[2] > node.bounds[0]) && (node.bounds[2] > node.bounds[1]))
                axis = 2;

            float currentCost = TraversalCost + node.bounds.GetSurfaceArea()*node.primitives.Count*IntersectionCost;

            // TODO: compute each split position only once
            float bestPosition = 0;
            float bestCost = float.PositiveInfinity;
            KDTreeNode bestLeft = null;
            KDTreeNode bestRight = null;
            foreach (Primitive primitive in node.primitives)
            {
                float minExtreme = primitive.GetMinExtreme(axis);
                float maxExtreme = primitive.GetMaxExtreme(axis);

                KDTreeNode outNodeLeft = null;
                KDTreeNode outNodeRight = null;

                float minCost = GetCost(node, axis, minExtreme, out outNodeLeft, out outNodeRight);
                if (minCost < bestCost)
                {
                    bestCost = minCost;
                    bestPosition = minExtreme;
                    bestLeft = outNodeLeft;
                    bestRight = outNodeRight;
                }

                float maxCost = GetCost(node, axis, maxExtreme, out outNodeLeft, out outNodeRight);
                if (maxCost < bestCost)
                {
                    bestCost = maxCost;
                    bestPosition = maxExtreme;
                    bestLeft = outNodeLeft;
                    bestRight = outNodeRight;
                }
            }

            if (bestCost >= currentCost)
            {
                return;
            }
            
            node.left = bestLeft;
            node.right = bestRight;

            Subdivide(bestLeft, depth+1);
            Subdivide(bestRight, depth+1);

        }

        private float GetCost (KDTreeNode node, int axis, float splitPos, out KDTreeNode left, out KDTreeNode right)
        {
            AABB[] bounds = node.bounds.Split(axis, splitPos);
            
            List<Primitive> minPrims = new List<Primitive>();
            List<Primitive> maxPrims = new List<Primitive>();

            foreach (Primitive primitive in node.primitives)
            {
                if (primitive.Intersects(bounds[0]))
                {
                    minPrims.Add(primitive);
                }
                if (primitive.Intersects(bounds[1]))
                {
                    maxPrims.Add(primitive);
                }
            }

            left = new KDTreeNode(bounds[0], minPrims);
            right = new KDTreeNode(bounds[1], maxPrims);

            return 2*TraversalCost +
                   IntersectionCost*(minPrims.Count*bounds[0].GetSurfaceArea() + maxPrims.Count*bounds[1].GetSurfaceArea());
        }

        public override bool FindIntersection (Ray r, out float closest, out Primitive closestPrimitive)
        {
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

    }
}
